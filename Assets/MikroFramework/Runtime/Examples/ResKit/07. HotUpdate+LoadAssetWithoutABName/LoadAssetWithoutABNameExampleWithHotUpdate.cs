using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.ResKit;
using UnityEngine;

namespace MikroFramework
{
    public class LoadAssetWithoutABNameExampleWithHotUpdate : HotUpdateEntranceManager {
        protected override void LaunchInDevelopingMode() {
            StartHotUpdate();
        }

        protected override void LaunchInTestingMode() {
            StartHotUpdate();
        }

        protected override void OnHotUpdateComplete() {
            
            Instantiate(resLoader.LoadSync<GameObject>("War loop"));
            
            AudioClip clip = resLoader.LoadSync<AudioClip>("War loop");
            Debug.Log($"Audio: {clip}");
        }
    }
}
