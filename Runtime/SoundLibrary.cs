using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Simple.SoundSystem.Core
{
    [CreateAssetMenu(fileName = "SoundLibrary", menuName = "SoundLibrary")]
    public class SoundLibrary : ScriptableObject
    {
        [FormerlySerializedAs("audioMixerGroup")] [SerializeField] public AudioMixerGroup AudioMixerGroup;
        [SerializeField] private List<Sound> _sounds = new List<Sound>();
        [SerializeField] private bool ignoreListenerPause = false;
        public bool IgnoreListenerPause { get => ignoreListenerPause; }
        public List<Sound> Sounds { get => _sounds; set => _sounds = value; }
        internal Sound FindSound(string name)
        {
            foreach (Sound data in Sounds)
            {
                if (data.Name == name)
                    return data;
            }

            return null;
        }

        private string[] GetAllSounds()
        {
            List<string> result = new List<string>();
            foreach (Sound data in Sounds)
            {
                result.Add(data.Name);
            }

            return result.ToArray();
        }

#if UNITY_EDITOR
        public static UnityEngine.Object EditorGetSoundData(string name)
        {
            SoundLibrary library = GetAllInstances<SoundLibrary>()[0];
            foreach (Sound data in library.Sounds)
            {
                if (data.Name == name)
                    return data as System.Object as UnityEngine.Object;
            }

            Debug.Log("Sound " + name + " not found!");

            return null;
        }
        public static string[] EditorGetSoundOptions()
        {
            SoundLibrary library = GetAllInstances<SoundLibrary>()[0];
            return library.GetAllSounds();
        }

        private static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;

        }

        [ContextMenu("AddNewEmptySoundEntry")]
        private void AddNewEmptySoundEntry()
        {
            AddNewSoundEntry();
        }

        public Sound AddNewSoundEntry(AudioClip clip = null)
        {
            Sound soundEntry = ScriptableObject.CreateInstance<Sound>();
            soundEntry.Initialise(this, clip);
            _sounds.Add(soundEntry);

            AssetDatabase.AddObjectToAsset(soundEntry, this);
            AssetDatabase.SaveAssets();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(soundEntry);

            return soundEntry;
        }

        [ContextMenu("Delete all")]
        private void DeleteAllSoundEntrys()
        {
            for (int i = _sounds.Count; i-- > 0;)
            {
                Sound tmp = _sounds[i];

                _sounds.Remove(tmp);
                Undo.DestroyObjectImmediate(tmp);
            }
            AssetDatabase.SaveAssets();
        }

#endif
    }
}