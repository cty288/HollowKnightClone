using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using UnityEngine;

namespace HollowKnight
{
    public class GameManager : AbstractMikroController<HollowKnight>, ISingleton {
        public bool DaggerGet = false;
        public static GameManager Singleton {
            get {
                return SingletonProperty<GameManager>.Singleton;
            }
        }

        private void Awake() {
            this.RegisterEvent<OnDaggerGet>(OnDaggerGet).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

         void ISingleton.OnSingletonInit() {
            
        }

        private void OnDaggerGet(OnDaggerGet eGet) {
            DaggerGet = true;
            Debug.Log("Dagger get");
        }
    }
}
