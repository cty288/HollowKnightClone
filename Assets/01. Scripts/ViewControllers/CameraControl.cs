using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public class ShakeCameraEvent {
        public float Duration;
        public float Strength = 3;
        public int Vibrato = 10;
        public float Randomness = 90;
    }

    public struct OnCutsceneCameraFirstMoveComplete {

    }

    public struct OnCutsceneCameraSecondMoveComplete
    {

    }

    public class CameraControl : AbstractMikroController<HollowKnight>, ICanSendEvent {
        private Player player;
        private Camera camera;


        [SerializeField]
        private float lerpSpeed = 20;

        [SerializeField] private float YOffset = 0;

        [SerializeField] private Vector2 cameraPositionXRange = new Vector2(0, 100);
        [SerializeField] private Vector2 cameraPositionYRange = new Vector2(0, 100);


        private bool inCutScene = false;
        private void Awake() {
            camera = GetComponent<Camera>();
            player = Player.Singleton;
        }

        private void Start() {
            this.RegisterEvent<ShakeCameraEvent>(OnCameraShake).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnCutSceneBorderLower>(OnCutSceneBorderLower).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnBossCutSceneComplete>(OnBossCutSceneComplete).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnBossCutSceneComplete(OnBossCutSceneComplete obj) {
            inCutScene = false;
        }

        private Tween cutSceneTween;

        private void OnCutSceneBorderLower(OnCutSceneBorderLower e) {
            inCutScene = true;
            this.GetSystem<ITimeSystem>().AddDelayTask(1.5f, () => {
                this.SendEvent<OnCutsceneCameraFirstMoveComplete>();
            });

            Vector3 moveBy = new Vector3(e.TargetCameraDest.x, transform.position.y,transform.position.z) - this.transform.position;

            cutSceneTween = transform.DOBlendableMoveBy(moveBy, 3f).OnComplete(() => {
               
                this.GetSystem<ITimeSystem>().AddDelayTask(e.CameraStopTime, () => {
                    this.SendEvent<OnCutsceneCameraSecondMoveComplete>();
                    transform.DOBlendableMoveBy(-moveBy, 3f).OnComplete(CutSceneCameraMoveComplete);
                    //inCutScene = false;
                });
            }).SetAutoKill(false);
            
        }

        private void CutSceneCameraMoveComplete() {
            cutSceneTween.Kill();
            this.SendEvent<OnBossCutSceneComplete>(new OnBossCutSceneComplete());
        }

        private void OnCameraShake(ShakeCameraEvent e) {
            camera.DOShakePosition(e.Duration, e.Strength, e.Vibrato, e.Randomness);
        }

        private void Update() {
            if (!inCutScene) {
                float targetX = transform.position.x;
                targetX = Mathf.Lerp(targetX, player.transform.position.x, lerpSpeed * Time.deltaTime);
                targetX = Mathf.Clamp(targetX, cameraPositionXRange.x, cameraPositionXRange.y);

                float targetY = transform.position.y;
                targetY = Mathf.Lerp(targetY, player.transform.position.y + YOffset, lerpSpeed * Time.deltaTime);
                targetY = Mathf.Clamp(targetY, cameraPositionYRange.x, cameraPositionYRange.y);

                transform.position = new Vector3(targetX, targetY, transform.position.z);
            }
          
        }
    }
}
