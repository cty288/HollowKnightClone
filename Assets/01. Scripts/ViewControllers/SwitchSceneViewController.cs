using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public class SwitchSceneViewController : AbstractMikroController<HollowKnight> {
        [SerializeField] private int switchToSceneNum = 1;
        [SerializeField] private Vector2 cameraXBound;
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject == Player.Singleton.gameObject) {
                GameManager.Singleton.SwitchScene(switchToSceneNum);
                this.GetSystem<ITimeSystem>().AddDelayTask(1f, () => {
                    Camera.main.GetComponent<CameraControl>().cameraPositionXRange = cameraXBound;
                });
               
            }
        }
    }
}
