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
        [SerializeField] private float damage = 0.5f;
        [SerializeField] private float force = 10;
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
                if (!other.isTrigger) {
                   
                    Debug.Log("hit");

                    if (other.gameObject.TryGetComponent<IEnemyViewControllerAttackable>(
                        out IEnemyViewControllerAttackable enemy)) {
                        if (enemy.Attackable.Health.Value > 0) {
                            enemy.Attackable.Attack(damage);
                            if (enemy.GameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody2d)) {
                                float moveRight = Target.x > transform.position.x ? 1 : -1;
                                


                                //Debug.Log($"Get rig. Velocity: {this.GetComponent<Rigidbody2D>().velocity.magnitude}");
                                rigidbody2d.AddForce(new Vector2(moveRight * force,0), ForceMode2D.Impulse);
                            }
                        }
                        else {
                            return;
                        }
                       
                    }

                    OnArrowReach();
                    Destroy(this.gameObject);
                }
               
            }
        }

        private void OnArrowReach() {
            this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.3f,0.7f,30,100));
            this.GetSystem<ITeleportSystem>().OnReachDest(transform.Find("PlayerSpawnPoint").transform.position);
        }
    }
}
