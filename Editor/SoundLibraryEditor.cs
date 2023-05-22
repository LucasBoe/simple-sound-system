using Simple.SoundSystem;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Simple.SoundSystem.Core;

namespace Simple.SoundSystem.Editor
{
    [CustomEditor(typeof(SoundLibrary))]
    public class SoundLibraryEditor : UnityEditor.Editor
    {
        SoundLibrary library;
        List<AudioClip> notIntegratedAudioClips = new List<AudioClip>();
        protected void OnEnable()
        {
            RefreshNotIntegratedAudioClips();
        }
        private void RefreshNotIntegratedAudioClips()
        {
            notIntegratedAudioClips.Clear();
            var guids = AssetDatabase.FindAssets("t:AudioClip");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                notIntegratedAudioClips.Add(clip);
            }

            library = target as SoundLibrary;

            foreach (var lib in LoadAllLibs())
            {
                foreach (var sound in lib.Sounds)
                {
                    if (sound != null)
                    {
                        if (sound.UseMultipleClipVariants)
                        {
                            foreach (var clip in sound.Clips)
                            {
                                if (clip != null && notIntegratedAudioClips.Contains(clip))
                                    notIntegratedAudioClips.Remove(clip);
                            }
                        }
                        else if (sound.Clip != null && notIntegratedAudioClips.Contains(sound.Clip))
                        {
                            notIntegratedAudioClips.Remove(sound.Clip);
                        }
                    }
                }
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Button("Create Empty Sound Element");
            GUILayout.Label("");

            bool refresh = false;

            GUILayout.Label("");
            var labelText = notIntegratedAudioClips.Count > 0 ? "AudioClips not integrated into sound system:" : "All AudioClips are part of a sound library.";
            GUILayout.Label(labelText);
            foreach (var sound in notIntegratedAudioClips)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Create Sound from: {sound.name}"))
                {
                    library.AddNewSoundEntry(sound);
                    refresh = true;
                }
                GUILayout.EndHorizontal();
            }

            if (refresh)
                RefreshNotIntegratedAudioClips();
        }

        private List<SoundLibrary> LoadAllLibs()
        {
            List<SoundLibrary> libs = new List<SoundLibrary>();
            var guids = AssetDatabase.FindAssets("t:SoundLibrary");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var lib = AssetDatabase.LoadAssetAtPath<SoundLibrary>(path);

                if (lib != null)
                    libs.Add(lib);

            }
            return libs;
        }
    }
}