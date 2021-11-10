using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.SceneEntranceKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MikroFramework.Examples
{
    public class HomeModule : EntranceManager
    {
        protected override void LaunchInDevelopingMode()
        {
            Debug.Log("Developing mode");
        }

        protected override void LaunchInReleasedMode()
        {
            Debug.Log("Released mode");
            //load resources
            //initialize sdk
            //start game
            GameModule.LoadModule();
        }

        protected override void LaunchInTestingMode()
        {
            Debug.Log("Testing mode");
            //test logics
            //load resources
            //initialize sdk
            //start game
            GameModule.LoadModule();
        }

    }

}