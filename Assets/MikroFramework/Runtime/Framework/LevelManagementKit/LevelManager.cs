using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MikroFramework.LevelManagementKit {
    [MonoSingletonPath("[FrameworkPersistent]/LevelManager")]
    public class LevelManager : ManagerBehavior, ISingleton {
        private List<string> levelNames;
        public int Index { get; set; }

        private static LevelManager singleton {
            get {
                LevelManager singleton = SingletonProperty<LevelManager>.Singleton;
                return singleton;
            }
        }

        void ISingleton.OnSingletonInit() {
            DontDestroyOnLoad(this.gameObject);
            Index = 0;
        }

        public static void Init(List<string> levelNames) {
            singleton.levelNames = levelNames;
        }

        public static void LoadCurrent() {
            SceneManager.LoadScene(singleton.levelNames[singleton.Index]);
        }

        public static void LoadNext() {
            singleton.Index++;
            if (singleton.Index >= singleton.levelNames.Count) {
                singleton.Index = 0;
            }

            SceneManager.LoadScene(singleton.levelNames[singleton.Index]);
        }

       
    }

}
