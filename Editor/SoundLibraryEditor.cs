using Simple.SoundSystem;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Simple.SoundSystem.Core;
using System.Text.RegularExpressions;

namespace Simple.SoundSystem.Editor
{
    [CustomEditor(typeof(SoundLibrary))]
    public class SoundLibraryEditor : UnityEditor.Editor
    {
        SoundLibrary library;
        List<AudioClip> notIntegratedAudioClips = new List<AudioClip>();
        List<string> foldedInPaths = new List<string>();
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

                if (!SoundSystemEditorUtil.ClipIsPartOfAnyLibrary(clip))
                    notIntegratedAudioClips.Add(clip);
            }

            library = target as SoundLibrary;
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

            Dictionary<string, List<AudioClip>> clipsSortedByFolder = new Dictionary<string, List<AudioClip>>();

            foreach (var sound in notIntegratedAudioClips)
            {
                var key = Regex.Match(AssetDatabase.GetAssetPath(sound), @"^.*[\/]").Value;

                if (!clipsSortedByFolder.ContainsKey(key))
                    clipsSortedByFolder.Add(key, new List<AudioClip>());

                clipsSortedByFolder[key].Add(sound);
            }

            foreach (var pair in clipsSortedByFolder)
            {
                var path = pair.Key;
                bool isFoldedOut = !foldedInPaths.Contains(path);

                bool shouldBeFoldedOut = EditorGUILayout.Foldout(isFoldedOut, path, toggleOnLabelClick: true);

                if (isFoldedOut != shouldBeFoldedOut)
                {
                    if (shouldBeFoldedOut)
                        foldedInPaths.Remove(path);
                    else
                        foldedInPaths.Add(path);
                }

                if (!isFoldedOut)
                    continue;

                foreach (var sound in pair.Value)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button($"Create Sound from: {sound.name}"))
                    {
                        library.AddNewSoundEntry(sound);
                        refresh = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            if (refresh)
                RefreshNotIntegratedAudioClips();
        }
    }
}