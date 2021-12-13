using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HollowKnight
{
    public class AudioManager : AbstractMikroController<HollowKnight> {
        [SerializeField] private AudioSource generalAudioSource;
        
        [SerializeField]
        private AudioSource absorbSystemAudioSource;

        [SerializeField] private AudioSource bgmAudioSource;

        [SerializeField] private AudioClip[] bgms;
        [SerializeField] private AudioClip bossBGM;

        [SerializeField] private AudioSource playerWalkAudioSource;
        [SerializeField] private AudioClip[] walkSounds;
        [SerializeField] private AudioClip[] runSounds;


        [SerializeField]
        private AudioSource attackSystemSource;
        [SerializeField] private AudioClip pointEnemySound;
        [SerializeField] private AudioClip normalAttackSound;
        [SerializeField] private AudioClip[] smallAnimalChargeSounds;

        [SerializeField] private AudioClip iMustSeekFurtherSound;


        public static AudioManager Singleton;

        private void Awake() {
            this.RegisterEvent<OnAbsorbInterrupted>(OnAbsorbInterrupted).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnAttackStartPrepare>(OnAttackPoint).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnNormalAttack>(OnNormalAttack).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnPlayerEnterBossRoom>(OnBossRoom).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnSceneSwitch>(OnSceneSwitch).UnRegisterWhenGameObjectDestroyed(gameObject);
            Singleton = this;
        }

        private void OnSceneSwitch(OnSceneSwitch e) {
            if (e.TargetScene == 1) {
                bgmAudioSource.clip = bgms[1];
            }else if (e.TargetScene == 2) {
                bgmAudioSource.clip = bgms[0];
            }

            bgmAudioSource.Play();
        }

        private void OnBossRoom(OnPlayerEnterBossRoom e) {
            bgmAudioSource.clip = bossBGM;
            bgmAudioSource.Play();
        }

        public void IMustSeekFurther() {
            PlayGeneralAudio(iMustSeekFurtherSound, 1);
        }
        
        public void PlayGeneralAudio(AudioClip audio, float volume)
        {
            generalAudioSource.PlayOneShot(audio, volume);
        }


        #region Attack
        private void OnAttackPoint(OnAttackStartPrepare obj) {
            attackSystemSource.PlayOneShot(pointEnemySound, 1);
        }

        private void OnNormalAttack(OnNormalAttack obj) {
            
            if (!attackSystemSource.isPlaying) {
                int chance = Random.Range(0, 100);
                if (chance <= 10)
                {
                    attackSystemSource.PlayOneShot(normalAttackSound, 1);
                }
            }
           
        }

        public void OnSmallAnimalStartCharge() {
            attackSystemSource.PlayOneShot(smallAnimalChargeSounds[0],1);
        }

        public void OnSmallAnimalNormal()
        {
            attackSystemSource.PlayOneShot(smallAnimalChargeSounds[2], 1);
        }

        public void OnSmallAnimalChargeReleased()
        {
            attackSystemSource.Stop();
            attackSystemSource.PlayOneShot(smallAnimalChargeSounds[1], 1);
        }

        public void OnThornShoot(AudioClip clip, float volume) {
            //if (!attackSystemSource.isPlaying) {
                attackSystemSource.PlayOneShot(clip, volume);
           // }
        }
        #endregion


        private void Start() {
            bgmAudioSource.clip = bgms[0];
            bgmAudioSource.Play();
        }

        #region Absorb
        private void OnAbsorbInterrupted(OnAbsorbInterrupted obj)
        {
            absorbSystemAudioSource.Stop();
        }

        public void PlayAbsorbAudio(AudioClip audio, float volume)
        {
            absorbSystemAudioSource.PlayOneShot(audio, volume);
        }
        #endregion


        #region Move
        public void OnWalk() {
            int index = Random.Range(0, walkSounds.Length);
            playerWalkAudioSource.PlayOneShot(walkSounds[index],1);
        }

        public void OnRun() {
            int index = Random.Range(0, runSounds.Length);
            playerWalkAudioSource.PlayOneShot(runSounds[index], 1);
        }

        public void OnWalkStop() {
            playerWalkAudioSource.Stop();
        }
        #endregion


        public void OnSingletonInit() {
            
        }
    }
}
