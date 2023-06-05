using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Simple.SoundSystem.Core
{
    public class Sound : ScriptableObject
    {
        [SerializeField] private SoundLibrary myLibrary;

        [SerializeField] AudioClip clip;
        public AudioClip Clip => clip;
        [SerializeField] bool useMultipleClipVariants = false;
        public bool UseMultipleClipVariants => useMultipleClipVariants;
        [SerializeField] List<AudioClip> clips = new List<AudioClip>();
        public List<AudioClip> Clips => clips;
        [SerializeField, Range(0, 1)] float volume = 0.5f;

        [SerializeField, Range(0, 3)] float pitch = 1f;
        [SerializeField] float pitchMin = 0.8f, pitchMax = 1.2f;
        [SerializeField] bool randomizePitch = false;
        public float AudioLength => clip.length;
        public float AudioVolume => volume;

        public SoundLibrary MyLibrary { get => myLibrary; }
        [SerializeField] private string customName = "";
        public string Name { get => customName != "" ? customName : (useMultipleClipVariants ? ((clips.Count > 0 && clips[0] != null) ? clips[0].name + "+++" : "New Sound") : (clip != null ? clip.name : "New Sound")); }

        public PlayingSound Play() { return SoundManager.Instance.Play(this); }
        public PlayingSound PlayLoop(float fadeDuration = 0.2f) { return SoundManager.Instance.Play(this, loop: true, fadeDuration); }

        public void PlaySound() => SoundManager.Instance.Play(this);
        public void Stop() => SoundManager.Instance.Stop(this);
        public void StopLoop() => Stop();

        internal float Configure(AudioSource audioSource, bool loop)
        {
            AudioClip c = useMultipleClipVariants ? clips[Random.Range(0, clips.Count)] : clip;
            audioSource.clip = c;
            audioSource.volume = volume;
            audioSource.pitch = randomizePitch ? Random.Range(pitchMin, pitchMax) : pitch;
            audioSource.loop = loop;
            audioSource.outputAudioMixerGroup = myLibrary.audioMixerGroup;

            return c.length;
        }

#if UNITY_EDITOR
        public void Initialise(SoundLibrary myLibrary, AudioClip clip = null)
        {
            this.myLibrary = myLibrary;
            this.clip = clip;
            Rename();
        }

        [ContextMenu("Update Name")]
        public void Rename()
        {
            this.name = Name;
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Delete this")]
        private void DeleteThis()
        {
            myLibrary.Sounds.Remove(this);
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}