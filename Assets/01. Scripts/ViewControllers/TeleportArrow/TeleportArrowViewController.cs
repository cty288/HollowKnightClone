using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Utilities;
using UnityEngine;

namespace HollowKnight
{
    public class TeleportArrowViewController : AbstractMikroController<HollowKnight> {
        public Vector2 Target;

        private float distToTargert;

        [SerializeField] private LayerMask layerMask;

        private Camera camera;

        private void Start() {
            this.RegisterEvent<OnTeleportInterrupted>(OnTeleportInterrupted)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            camera = Camera.main;
        }

        private void OnTeleportInterrupted(OnTeleportInterrupted e) {
            OnArrowReach();
            Destroy(this.gameObject);
        }

        private void Update() {
            distToTargert = Mathf.Abs(Vector2.Distance(transform.position, Target));
            if (distToTargert < 0.2f) {
                OnArrowReach();
                Destroy(this.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (PhysicsUtility.IsInLayerMask(other.gameObject, layerMask)) {
                OnArrowReach();
                
                Debug.Log("hit");
                Destroy(this.gameObject);
            }
        }

        private void OnArrowReach() {
            camera.DOShakePosition(0.3f, 0.7f, 30, 100);
            this.GetSystem<ITeleportSystem>().OnReachDest(transform.Find("PlayerSpawnPoint").transform.position);
        }
    }
}
