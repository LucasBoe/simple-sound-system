using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Simple.SoundSystem.Core
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] SoundValidator validator;
        public static SoundManager Instance => instance;
        private static SoundManager instance;

        Dictionary<PlayingSound, Sound> playingSounds = new Dictionary<PlayingSound, Sound>();
        CustomSpacialTargetHandler customTargets = new CustomSpacialTargetHandler();

        [SerializeField] bool log;

        protected virtual void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
                instance = this;
        }

        internal PlayingSound Play(Sound soundData, SoundParameters parameters = null)
        {
            PlayingSound playing = new PlayingSound();

            if (validator != null && soundData.NeedsValidation)
            {
                if (parameters.IsSpacialSound)
                {
                    var target = customTargets.GetCustomTarget(parameters, playing);
                    var pos = target.Object != null ? target.Object.transform.position : target.PosRef.Value;

                    if (!validator.ValidateSound3D(soundData, pos))
                        return null;
                }
                else
                {
                    if (!validator.ValidateSound(soundData))
                        return null;
                }
            }

            if (parameters == null)
                parameters = new SoundParameters();

            playing.SoundData = soundData;
            playing.Parameters = parameters;
            playing.Volume = soundData.AudioVolume * parameters.CustomVolumeMultiplier;
            playing.CustomTarget =
                parameters.IsSpacialSound ? customTargets.GetCustomTarget(parameters, playing) : null;

            var host = playing.CustomTarget != null ? playing.CustomTarget.Object : gameObject;
            playing.AudioSource = host.AddComponent<AudioSource>();
            playing.Coroutine = StartCoroutine(PlaySoundRoutine(soundData, playing, parameters));
            playingSounds.Add(playing, soundData);

            Log("Play sound " + soundData.Name + " looping: " + parameters.Loop);

            if (parameters.Loop && parameters.AlsoFadeInNotOnlyOut)
                FadeIn(playing.AudioSource, playing.Volume, parameters.FadeDuration, soundData.MyLibrary.IgnoreListenerPause);

            return playing;
        }

        private void Log(string message)
        {
            if (log) Debug.Log(message);
        }

        internal void Stop(Sound sound)
        {
            Log("stop sound " + sound.Name);
            List<PlayingSound> toStop = new List<PlayingSound>();

            foreach (KeyValuePair<PlayingSound, Sound> playingSound in playingSounds)
            {
                if (playingSound.Value == sound)
                    toStop.Add(playingSound.Key);
            }

            for (int i = toStop.Count - 1; i >= 0; i--)
            {
                toStop[i].Stop();
            }
        }

        internal void Stop(PlayingSound sound)
        {
            sound.Stop();
        }

        private IEnumerator PlaySoundRoutine(Sound data, PlayingSound playing, SoundParameters parameters)
        {
            float length = data.Configure(playing, parameters);
            playing.AudioSource.Play();

            if (!parameters.Loop)
            {
                if (data.MyLibrary.IgnoreListenerPause)
                    yield return new WaitForSecondsRealtime(length);
                else
                    yield return new WaitForSeconds(length);

                playing.Stop();
            }
        }

        internal void FadeIn(AudioSource audioSource, float volume, float fadeDuration,  bool useUnscaledTime) =>
            StartCoroutine(VolumeBlendRoutine(audioSource, 0f, volume, fadeDuration, useUnscaledTime, null));

        internal void FadeOut(AudioSource audioSource, float volume, float fadeDuration, bool useUnscaledTime, Action onFinished) =>
            StartCoroutine(VolumeBlendRoutine(audioSource, volume, 0f, fadeDuration, useUnscaledTime, onFinished));

        private IEnumerator VolumeBlendRoutine(AudioSource audioSource, float from, float to, float fadeDuration,
            bool useUnscaledTime, Action onFinished)
        {

            float t = 0f;
            while (t < 1f && audioSource != null)
            {
                t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime / fadeDuration;
                float volume = Mathf.Lerp(from, to, t);
                audioSource.volume = volume;
                yield return null;
            }

            if (audioSource != null)
                audioSource.volume = to;

            onFinished?.Invoke();
        }

        internal void RemoveFromPlaying(PlayingSound playingAudio) => playingSounds.Remove(playingAudio);

        internal void DestroyPlayingInstance(PlayingSound playingSound)
        {
            if (playingSound.CustomTarget != null)
            {
                customTargets.RemoveFromCustomTarget(playingSound);
            }

            Destroy(playingSound.AudioSource);
        }
    }

    public class PlayingSound
    {
        public Sound SoundData;
        public AudioSource AudioSource;
        public Coroutine Coroutine;
        public SoundParameters Parameters;
        public float Volume;
        internal CustomSpacialTarget CustomTarget;

        public void Stop()
        {
            SoundManager soundManager = SoundManager.Instance;

            if (Coroutine != null)
                soundManager.StopCoroutine(Coroutine);

            soundManager.RemoveFromPlaying(this);
            soundManager.FadeOut(AudioSource, Volume, Parameters.FadeDuration, SoundData.MyLibrary.IgnoreListenerPause,
                () => { soundManager.DestroyPlayingInstance(this); });
        }
    }
}