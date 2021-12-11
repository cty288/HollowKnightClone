using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public struct OnCutSceneBorderLower {
        public Vector2 TargetCameraDest;
        public float CameraStopTime;
    }

    public struct OnBossCutSceneComplete {

    }

    public class BossCutscene : AbstractMikroController<HollowKnight>, ICanSendEvent {
        private Animator animator;

        [SerializeField] private Vector2 targetCameraDest;
        [SerializeField] private float cameraStopTime;
        

        private void Awake() {
            animator = GetComponent<Animator>();

            this.RegisterEvent<OnPlayerEnterBossRoom>(OnPlayerEnterBossRoom)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnCutsceneCameraSecondMoveComplete>(OnCutsceneCameraSecondMoveComplete)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnCutsceneCameraSecondMoveComplete(OnCutsceneCameraSecondMoveComplete obj) {
            animator.SetTrigger("Lower");
        }

        private void OnPlayerEnterBossRoom(OnPlayerEnterBossRoom e) {
            animator.SetTrigger("CutSceneStart");
            Player.Singleton.FrozePlayer(true);
        }

        public void OnFirstStageCutSceneFinished() {
            this.SendEvent<OnCutSceneBorderLower>(new OnCutSceneBorderLower() {
                CameraStopTime = cameraStopTime,
                TargetCameraDest = targetCameraDest
            });
        }
    }
}
