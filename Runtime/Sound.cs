using System;
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
        public PlayingSound Play(
            bool loop = false,
            float fadeDuration = 0.2f,
            float volumeMultiplier = 1f,
            Vector3? customPosition = null,
            Transform customTarget = null,
            float customRange = 7f,
            bool loopRandomizeStartTime = true,
            bool alsoFadeInNotOnlyOut = true
            )
        {
            return SoundManager.Instance.Play(this, new SoundParameters()
            {
                Loop = loop,
                FadeDuration = fadeDuration,
                CustomVolumeMultiplier = volumeMultiplier,
                CustomSpacialPosition = customPosition,
                CustomSpacialTransformTarget = customTarget,
                CustomSpacialRange = customRange,
                LoopRandomizeStartTime = loopRandomizeStartTime,
                AlsoFadeInNotOnlyOut = alsoFadeInNotOnlyOut
            });
        }
        public PlayingSound Play(SoundParameters parameters) { return SoundManager.Instance.Play(this, parameters); }
        public PlayingSound PlayAt(Transform transform, float range = 7f) { return SoundManager.Instance.Play(this, new SoundParameters() { CustomSpacialTransformTarget = transform, CustomSpacialRange = range }); }
        public PlayingSound PlayAt(Vector3 position, float range = 7f) { return SoundManager.Instance.Play(this, new SoundParameters() { CustomSpacialPosition = position, CustomSpacialRange = range }); }
        public PlayingSound PlayLoop(float fadeDuration = 0.2f) { return SoundManager.Instance.Play(this, new SoundParameters() { Loop = true, FadeDuration = fadeDuration }); }

        public void PlaySound() => SoundManager.Instance.Play(this);
        public void Stop() => SoundManager.Instance.Stop(this);
        public void StopLoop() => Stop();

        internal float Configure(PlayingSound playing, SoundParameters parameters)
        {
            AudioSource audioSource = playing.AudioSource;
            audioSource.Stop();

            AudioClip c = useMultipleClipVariants ? clips[UnityEngine.Random.Range(0, clips.Count)] : clip;
            audioSource.clip = c;
            audioSource.volume = playing.Volume;
            audioSource.pitch = randomizePitch ? UnityEngine.Random.Range(pitchMin, pitchMax) : pitch;
            audioSource.loop = parameters.Loop;
            bool setRandomLoopTime = parameters.Loop && parameters.LoopRandomizeStartTime && clip.length > 1f; //if the clip is too short, unity errors occur
            audioSource.time = setRandomLoopTime ? UnityEngine.Random.Range(0f, clip.length) : 0f;
            audioSource.outputAudioMixerGroup = myLibrary.audioMixerGroup;
            audioSource.spatialBlend = parameters.IsSpacialSound ? 1 : 0;
            audioSource.maxDistance = parameters.CustomSpacialRange;
            audioSource.rolloffMode = AudioRolloffMode.Linear;

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

        public bool ContainsClip(AudioClip clip)
        {
            if (!useMultipleClipVariants)
                return clip == this.clip;

            return clips.Contains(clip);
        }
#endif
    }
}