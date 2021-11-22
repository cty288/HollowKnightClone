using System;
using System.Collections;
using System.Collections.Generic;
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

        private void Start() {
            this.RegisterEvent<OnTeleportInterrupted>(OnTeleportInterrupted)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
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
            this.GetSystem<ITeleportSystem>().OnReachDest(transform.Find("PlayerSpawnPoint").transform.position);
        }
    }
}
