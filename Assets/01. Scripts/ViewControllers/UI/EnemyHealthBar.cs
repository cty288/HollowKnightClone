using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MikroFramework;
using TMPro;
using MikroFramework.Architecture;
using MikroFramework.Event;

namespace HollowKnight {
	public partial class EnemyHealthBar : AbstractMikroController<HollowKnight> {
        private IEnemyViewControllerAttackable parentAttackable;

        private void Awake() {
            parentAttackable = this.GetComponentInParent<IEnemyViewControllerAttackable>();
           
        }

        private void Update() {
            if (parentAttackable.GameObject.transform.lossyScale.x == 1) {
                transform.DOScaleX(1,0);
            }
            else {
                transform.DOScaleX(-1, 0);
            }
        }

        private void Start() {
            parentAttackable.Attackable.Health.RegisterOnValueChaned(OnHealthChange).
                UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnPlayerRespawned>(OnRespawn).UnRegisterWhenGameObjectDestroyed(gameObject);
            OnHealthChange(0, parentAttackable.Attackable.Health.Value);
        }

        private void OnRespawn(OnPlayerRespawned e) {
            OnHealthChange(0, parentAttackable.Attackable.MaxHealth);
        }

        private void OnHealthChange(float oldHealth, float newHealth) {
            if (newHealth >= parentAttackable.Attackable.MaxHealth) {
                SliderEnemyHealth.gameObject.SetActive(false);
            }
            else {
                SliderEnemyHealth.gameObject.SetActive(true);
            }
            DOTween.To(() => SliderEnemyHealth.value,
                x => SliderEnemyHealth.value = x, newHealth / parentAttackable.Attackable.MaxHealth,
                0.2f);
            

            if (newHealth <= 0) {
                SliderEnemyHealth.gameObject.SetActive(false);
            }
        }
    }
}