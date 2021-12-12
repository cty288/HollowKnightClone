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

        private void Start() {
            parentAttackable.Attackable.Health.RegisterOnValueChaned(OnHealthChange).
                UnRegisterWhenGameObjectDestroyed(gameObject);

            OnHealthChange(0, parentAttackable.Attackable.Health.Value);
        }

        private void OnHealthChange(float oldHealth, float newHealth) {
            DOTween.To(() => SliderEnemyHealth.value,
                x => SliderEnemyHealth.value = x, newHealth / parentAttackable.Attackable.MaxHealth,
                0.2f);
            

            if (newHealth <= 0) {
                this.gameObject.SetActive(false);
            }
        }
    }
}