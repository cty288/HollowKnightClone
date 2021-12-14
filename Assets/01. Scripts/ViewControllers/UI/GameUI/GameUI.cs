using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.TimeSystem;

namespace HollowKnight {
	public partial class GameUI : AbstractMikroController<HollowKnight> {
        private float ultTime = 0;
        private float maxUltTime = 0;
        private WeaponType ultWeaponType;

        

        [SerializeField] private Sprite fullSprite;
        [SerializeField] private Sprite notFullSprite;

        [SerializeField] private GameObject gameEndCanvas;

        [SerializeField] private DialogueUIController speeDialogueUiController;

        [SerializeField] private GameObject StartGameUI;
        [SerializeField] private GameObject healthSlides;
        private void Start() {
            this.GetModel<IPlayerModel>().Health.RegisterOnValueChaned(OnHealthChange).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetModel<IPlayerModel>().UltChargeAccumlated.RegisterOnValueChaned(OnUltChange)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnUltChargeToMax>(OnUltChargeToMax).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnPlayerEnterBossRoom>(OnPlayerEnterBossRoom).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnBossHurt>(OnBossHurt).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnUltAttack>(OnUltAttackStart).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnCutsceneCameraSecondMoveComplete>(OnCutsceneCameraSecondMoveComplete)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnPlayerRespawned>(OnRespawn).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnBossDie>(OnBossDie).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnGameStart>(OnGameStart).UnRegisterWhenGameObjectDestroyed(gameObject);
            

            OnUltChange(0,0);
            OnHealthChange(0,100);
        }

        private void OnGameStart(OnGameStart obj) {
            StartGameUI.GetComponent<Animation>().Play();
            this.GetSystem<ITimeSystem>().AddDelayTask(1, () => {
                healthSlides.GetComponent<Animation>().Play();
            });
        }

        private void OnBossDie(OnBossDie e) {
            Player.Singleton.FrozePlayer(true);
            SliderBossHealth.gameObject.SetActive(false);
            CutSceneBars.Singleton.StartBars(1, () => {
                this.GetSystem<ITimeSystem>().AddDelayTask(2, () => {

                    speeDialogueUiController.ShowDialogueWithTypewriter("", "I must seek further...",
                        null, null);
                    AudioManager.Singleton.IMustSeekFurther();
                });

                this.GetSystem<ITimeSystem>().AddDelayTask(5, () => {
                    gameEndCanvas.gameObject.SetActive(true);
                    GameManager.Singleton.EndGame();
                });
            });
        }

        private void OnRespawn(OnPlayerRespawned obj) {
            OnHealthChange(0, this.GetModel<IPlayerConfigurationModel>().MaxHealth);
            OnBossHurt(new OnBossHurt(){currentHealth = 100, maxHealth = 100});
        }

        private void OnCutsceneCameraSecondMoveComplete(OnCutsceneCameraSecondMoveComplete obj) {
            
            SliderBossHealth.gameObject.SetActive(true);
        }

        public void Test() {
            Debug.Log("23333");
        }

        private void OnUltAttackStart(OnUltAttack e) {
            this.ultTime = e.LastTime;
            this.maxUltTime = e.LastTime;
            ultWeaponType = e.WeaponType;
        }

        private void Update() {
            if (ultTime > 0) {
                ultTime -= Time.deltaTime;
                ultTime = Mathf.Max(0, ultTime);

                if (this.GetSystem<IWeaponSystem>().SelectedWeapon == null
                    || this.GetSystem<IWeaponSystem>().SelectedWeapon.Type.Value == ultWeaponType) {

                    SliderUltCharge.value = ultTime / maxUltTime;
                    ImgBackgroundFull.enabled = true;
                    ImgBackgroundNotFull.enabled = false;
                    ImgFill.sprite = fullSprite;
                }
                else {
                    SliderUltCharge.value = this.GetModel<IPlayerModel>().UltChargeAccumlated.Value
                                            / this.GetModel<IPlayerConfigurationModel>().MaxUltChargeNeeded;
                    ImgBackgroundFull.enabled = false;
                    ImgBackgroundNotFull.enabled = true;
                    ImgFill.sprite = notFullSprite;
                }
            }
            else {
                DOTween.To(() => SliderUltCharge.value,
                    x => SliderUltCharge.value = x, this.GetModel<IPlayerModel>().UltChargeAccumlated.Value
                                                    / this.GetModel<IPlayerConfigurationModel>().MaxUltChargeNeeded,
                    0.2f);
                

                if (SliderUltCharge.value < 1) {
                    ImgBackgroundFull.enabled = false;
                    ImgBackgroundNotFull.enabled = true;
                    ImgFill.sprite = fullSprite;
                }
                else {
                    ImgBackgroundFull.enabled = true;
                    ImgBackgroundNotFull.enabled = false;
                    ImgFill.sprite = notFullSprite;
                }
            }
           
        }

        private void OnBossHurt(OnBossHurt e) {
            Debug.Log("Boss hurt");

            DOTween.To(() => SliderBossHealth.value,
                x => SliderBossHealth.value = x, e.currentHealth / e.maxHealth,
                0.2f);

            if (e.currentHealth <= 0) {
                SliderBossHealth.value = 0;
            }
        }

        private void OnPlayerEnterBossRoom(OnPlayerEnterBossRoom e) {
            
        }

        private void OnUltChargeToMax(OnUltChargeToMax e) {
            ImgBackgroundFull.enabled = true;
            ImgBackgroundNotFull.enabled = false;
            ImgFill.sprite = fullSprite;
        }

        private void OnUltChange(float oldUlt, float newUlt) {
            if (ultTime <= 0) {
                DOTween.To(() => SliderUltCharge.value,
                    x => SliderUltCharge.value = x, this.GetModel<IPlayerModel>().UltChargeAccumlated.Value
                                                    / this.GetModel<IPlayerConfigurationModel>().MaxUltChargeNeeded,
                    0.2f);
                if (SliderUltCharge.value < 1)
                {
                    ImgBackgroundFull.enabled = false;
                    ImgBackgroundNotFull.enabled = true;
                    ImgFill.sprite = notFullSprite;
                }
            }
        }

        private void OnHealthChange(float oldHealth, float newHealth) {
            DOTween.To(() => SliderPlayerHealth.value,
                x => SliderPlayerHealth.value = x, newHealth / this.GetModel<IPlayerConfigurationModel>().MaxHealth,
                0.2f);
        }
    }
}