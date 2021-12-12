using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public class DieBG : AbstractMikroController<HollowKnight> {
        private Animation animation;

        private void Awake() {
            animation = GetComponent<Animation>();
            this.RegisterEvent<OnPlayerDie>(OnPlayerDie).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnPlayerDie(OnPlayerDie obj) {
            this.GetSystem<ITimeSystem>().AddDelayTask(1, () => {
                animation.Play();
            });

        }

        public void OnDieBGBlack() {
            GameManager.Singleton.Respawn();
        }
    }
}
