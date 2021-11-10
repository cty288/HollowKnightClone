using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class MonoSingletonPathExample : MonoBehaviour {
        [MonoSingletonPath("[Logic]/GameManager")]
        public class GameManager : MonoMikroSingleton<GameManager> { }

        [MonoSingletonPath("[Framework]/ResManager]")]
        public class ResManager : MonoMikroSingleton<ResManager> { }

        [MonoSingletonPath("[Framework]/UIManager")]
        public class UIManager : MonoPersistentMikroSingleton<UIManager> { }

        [MonoSingletonPath("[Framework]/SoundManager")]
        public class SoundManager : MonoMikroSingleton<SoundManager> { }


        [MonoSingletonPath("[Logic]/ConfigManager")]
        public class ConfigManager : MonoMikroSingleton<ConfigManager> { }

        [MonoSingletonPath("[Others]/NetworkManager")]
        public class NetworkManager : MonoPersistentMikroSingleton<NetworkManager> { }

        [MonoSingletonPath("[SingletonProperty]/Test")]
        public class SingletonPathSingletonProperty: MonoBehaviour, ISingleton {
            public static SingletonPathSingletonProperty Singleton {
                get {
                    return SingletonProperty<SingletonPathSingletonProperty>.Singleton;
                }
            }

            private SingletonPathSingletonProperty() { }
            public void OnSingletonInit() {
                
            }
        }

        private void Start() {
            var gameMgr = GameManager.Singleton;

            var resMgr = ResManager.Singleton;

            var uiMgr = UIManager.Singleton;

            var soundMgr = SoundManager.Singleton;

            var congfigMgr = ConfigManager.Singleton;

            var networkMgr = NetworkManager.Singleton;

            var singletonPropertyTest = SingletonPathSingletonProperty.Singleton;
        }
    }
}
