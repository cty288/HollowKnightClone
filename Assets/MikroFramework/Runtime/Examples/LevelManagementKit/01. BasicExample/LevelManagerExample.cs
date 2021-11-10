using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.LevelManagementKit;
using MikroFramework.Managers;
using UnityEngine;

namespace MikroFramework.Examples {
    public class LevelManagerExample : MikroBehavior
    {
        protected override void OnBeforeDestroy()
        {

        }
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/LevelManagemmentKit/01. BasicExample", false,1)]
        private static void MenuClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject("LevelExample").AddComponent<LevelManagerExample>();
        }
#endif

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
            LevelManager.Init(new List<string>() {
                "HomeExample",
                "GameTest"
            });

            LevelManager.LoadCurrent();
            
            Delay(10f,LevelManager.LoadNext);
        }

    }

}
