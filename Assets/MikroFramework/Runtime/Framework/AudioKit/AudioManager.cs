using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.AudioKit {
    [MonoSingletonPath("[FrameworkPersistent]/AudioManager")]
    public class AudioManager : ManagerBehavior, ISingleton {

        private static AudioManager singleton {
            get {
                return SingletonProperty<AudioManager>.Singleton;
            }
        }

        private AudioListener audioListener;


        /// <summary>
        /// Play an audio clip
        /// </summary>
        /// <param name="audioName"></param>
        public static void PlayAudio(string audioName)
        {
            singleton.CheckAudioListener();

            AudioClip clip = Resources.Load<AudioClip>(audioName);
            AudioSource audioSource = singleton.gameObject.AddComponent<AudioSource>();

            audioSource.clip = clip;
            audioSource.Play();
        }

        private AudioSource bgmAudioSource = null;

        /// <summary>
        /// Play a background music (a scene can only have one bgm at a time)
        /// </summary>
        /// <param name="musicName"></param>
        /// <param name="loop"></param>
        public static void PlayBGM(string musicName,bool loop)
        {
            singleton.CheckAudioListener();

            if (!singleton.bgmAudioSource) {
                singleton.bgmAudioSource = singleton.gameObject.AddComponent<AudioSource>();
            }
            AudioClip clip = Resources.Load<AudioClip>(musicName);

            singleton.bgmAudioSource.clip = clip;
            singleton.bgmAudioSource.loop = loop;
            singleton.bgmAudioSource.Play();
        }

        /// <summary>
        /// Stop playing the current BGM
        /// </summary>
        public static void StopBGM() {
            singleton.bgmAudioSource.Stop();
        }


        /// <summary>
        /// Pause playing the current BGM
        /// </summary>
        public static void PauseBGM() {
            singleton.bgmAudioSource.Pause();
        }

        /// <summary>
        /// Resume the paused BGM
        /// </summary>
        public static void ResumeBGM() {
            singleton.bgmAudioSource.UnPause();
        }

        /// <summary>
        /// Mute the BGM
        /// </summary>
        public static void BGMMute() {
            singleton.bgmAudioSource.Pause();
            singleton.bgmAudioSource.mute = true;
        }

        /// <summary>
        /// Turn off all audios
        /// </summary>
        public static void AudioOff() {
            AudioSource[] audioSources = singleton.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource audioSource in audioSources) {
                if (audioSource != singleton.bgmAudioSource) {
                    audioSource.Pause();
                    audioSource.mute = true;
                }
            }
        }

        /// <summary>
        /// Turn on the BGM
        /// </summary>
        public static void BGMOn()
        {
            singleton.bgmAudioSource.UnPause();
            singleton.bgmAudioSource.mute = false;
        }
        /// <summary>
        /// Turn on all audios
        /// </summary>
        public static void AudioOn()
        {
            AudioSource[] audioSources = singleton.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource != singleton.bgmAudioSource)
                {
                    audioSource.UnPause();
                    audioSource.mute = false;
                }
            }
        }

        private void CheckAudioListener()
        {
            if (!audioListener) {
                audioListener = FindObjectOfType<AudioListener>();

                if (!audioListener) {
                    audioListener = gameObject.AddComponent<AudioListener>();
                }
                
            }
        }

        void ISingleton.OnSingletonInit() {
            
        }
    }

}
