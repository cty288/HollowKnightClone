using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.SceneEntranceKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MikroFramework.Examples {
    public class GameModule : EntranceManager
    {
        public static void LoadModule() {
            SceneManager.LoadScene("GameTest");
        }
        protected override void LaunchInDevelopingMode() {
            //load resource
            //initialize sdk
            //start game
            Debug.Log("Developing mode");
        }

        protected override void LaunchInReleasedMode()
        {
            Debug.Log("Released mode");
        }

        protected override void LaunchInTestingMode()
        {
            Debug.Log("Testing mode");
        }

    }

}
