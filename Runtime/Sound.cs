using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoundSystem
{
    public class Sound : ScriptableObject
    {
        [SerializeField, ReadOnly] private SoundLibrary _myLibrary;
        [SerializeField, OnValueChanged("Rename")] AudioClip clip;
        [SerializeField, Range(0,1)] float volume = 0.5f;

        [SerializeField, Range(0, 3), HideIf("randomizePitch")] float pitch = 1f;
        [SerializeField, MinMaxSlider(0f, 3f), ShowIf("randomizePitch")] Vector2 pitchRange = new Vector2(0.75f, 1.25f);
        [SerializeField] bool randomizePitch = false;
        public float AudioLength => clip.length;


        public SoundLibrary MyLibrary { get => _myLibrary; }
        public string Name { get => clip.name; }

        internal void Play()
        {
            SoundManager.Instance.Play(this);
        }

        internal void Play(Vector3 position)
        {
            SoundManager.Instance.Play(this, position);
        }

        internal void Configure(AudioSource audioSource)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = randomizePitch ? Random.Range(pitchRange.x, pitchRange.y) : pitch;
        }

#if UNITY_EDITOR
        public void Initialise(SoundLibrary mryLibrary)
        {
            _myLibrary = mryLibrary;
        }

        [ContextMenu("Rename to name")]
        private void Rename()
        {
            this.name = Name;
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Delete this")]
        private void DeleteThis()
        {
            _myLibrary.Sounds.Remove(this);
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}