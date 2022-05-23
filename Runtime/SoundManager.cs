using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class SoundManager : SingletonBehaviour<SoundManager>
    {
        private IEnumerator PlaySoundRoutine(Sound data)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            data.Configure(audioSource);
            audioSource.Play();

            yield return new WaitForSeconds(data.AudioLength);

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
