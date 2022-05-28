using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance => instance;
        private static SoundManager instance;

        protected virtual void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
                instance = this;
        }

        private IEnumerator PlaySoundRoutine(Sound data)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            float length = data.Configure(audioSource);
            audioSource.Play();

            yield return new WaitForSeconds(length);

            Destroy(audioSource);
        }

        internal void Play(Sound soundData)
        {
            StartCoroutine(PlaySoundRoutine(soundData));
        }

        internal void Play(Sound soundData, Vector3 position)
        {
            Play(soundData);
        }
    }
}
