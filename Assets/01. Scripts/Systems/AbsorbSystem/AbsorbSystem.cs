using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public struct OnEnemyAbsorbPreparing {
        public GameObject absorbedEnemy;
    }

    public struct OnEnemyAbsorbing {
        public GameObject absorbedEnemy;
        public float absorbPercentage;
    }

    public struct OnEnemyAbsorbed {
        public GameObject absorbedEnemy;
    }

    public struct OnAbsorbInterrupted {
        public GameObject absorbedEnemy;
    }

    public interface IAbsorbSystem : ISystem {
        public AbsorbState AbsorbState { get; }

        public GameObject AbsorbingGameObject { get; }
        bool Absorb(Vector2 mousePosition);

        public float CutSelfTime { get; }

        public float AbsorbPrepareTime { get; }
        public float AbsorbingTime { get; }

        public float MaxAbsorbDistance { get; }

        public void AbsorbInterrupt();
    }

    public enum AbsorbState {
        NotAbsorbing,
        AbsorbPreparing,
        Absorbing,
    }

    public class AbsorbSystem : AbstractSystem, IAbsorbSystem {
        private AbsorbState absorbState;
        private TimeSystem timeSystem;

        private bool cutSelfTriggered = false;
        private bool absorbPrepareFinished = false;
        
        private Camera cam;
        private float timer;

        private GameObject lastAbsorbObj;
        private GameObject absorbingObject;

      


    protected override void OnInit() {
            absorbState = AbsorbState.NotAbsorbing;
            cam = Camera.main;
            timeSystem = new TimeSystem();
            timeSystem.Start();
        }

        public AbsorbState AbsorbState {
            get {
                return absorbState;
            }
        }
        public GameObject AbsorbingGameObject {
            get { return absorbingObject; }
        }
        private LayerMask Mask = LayerMask.GetMask("Enemy","EnemyTraversable");
        public bool Absorb(Vector2 mousePosition) {

            RaycastHit2D ray = Physics2D.GetRayIntersection(cam.ScreenPointToRay(mousePosition),1000, Mask);

            Collider2D collider = ray.collider;

            IEnemyViewController component = null;

            Vector2 mousePos = cam.ScreenToWorldPoint(mousePosition);
            
            if ((collider != null && collider.TryGetComponent<IEnemyViewController>(out component))
                && Mathf.Abs(Vector2.Distance(mousePos, Player.Singleton.transform.position))<=MaxAbsorbDistance) {
                
                if ((component!=null && component.CanAbsorb)) {
                    
                   
                    timer += Time.deltaTime;
                    absorbingObject = collider.gameObject;


                    if (lastAbsorbObj != null) {
                        if (absorbingObject != lastAbsorbObj) {
                            //AbsorbInterrupt();
                            //Debug.Log($"Not equal. Absorbing: {absorbingObject.gameObject.name}, Last: {lastAbsorbObj.gameObject.name}");
                            absorbingObject = lastAbsorbObj;
                            //Reset();
                           // return false;
                        }
                    }

                    lastAbsorbObj = absorbingObject;
                    
                    
                    if (!absorbPrepareFinished) {
                        absorbPrepareFinished = true;
                        this.SendEvent<OnEnemyAbsorbPreparing>(new OnEnemyAbsorbPreparing() { absorbedEnemy = collider.gameObject });
                        absorbState = AbsorbState.AbsorbPreparing;
                    }
                    

                    if (timer >= CutSelfTime && !cutSelfTriggered) {
                        cutSelfTriggered = true;
                        this.SendEvent<ShakeCameraEvent>(new ShakeCameraEvent()
                        {
                            Duration = 0.1f,
                            Randomness = 100,
                            Strength = 0.3f,
                            Vibrato = 20
                        });
                    }


                    //Debug.Log(timer);
                    if (timer >= AbsorbPrepareTime && timer <= AbsorbPrepareTime + AbsorbingTime) {
                        this.SendEvent<OnEnemyAbsorbing>(new OnEnemyAbsorbing() {
                            absorbedEnemy = absorbingObject.gameObject,
                            absorbPercentage = (timer - AbsorbPrepareTime) / AbsorbingTime
                        });
                        absorbState = AbsorbState.Absorbing;
                    }

                    if (timer >= AbsorbingTime + AbsorbPrepareTime) {
                        this.SendEvent<OnEnemyAbsorbed>(new OnEnemyAbsorbed() { absorbedEnemy = absorbingObject.gameObject });
                        absorbState = AbsorbState.NotAbsorbing;
                        
                        Reset();
                    }

                    return true;
                }
                else {
                    AbsorbInterrupt();
                }
            }
            else {
                AbsorbInterrupt();
            }

            return false;
        }

        public float CutSelfTime { get; } = 0.9f;

        public float AbsorbPrepareTime { get; } = 1.4f;
        public float AbsorbingTime { get; } = 1f;
        public float MaxAbsorbDistance { get; } = 7f;

        public void AbsorbInterrupt() {
            if (absorbingObject != null) {
                if (absorbState != AbsorbState.NotAbsorbing) {
                    Debug.Log("Interrupted");
                    Debug.Log(absorbingObject.gameObject.name);

                    absorbState = AbsorbState.NotAbsorbing;
                    this.SendEvent<OnAbsorbInterrupted>(new OnAbsorbInterrupted() { absorbedEnemy = absorbingObject });
                    Reset();
                }
                
            }
        }

        public void Reset() {
            absorbState = AbsorbState.NotAbsorbing;
            timer = 0;
            lastAbsorbObj = null;
            absorbingObject = null;
            cutSelfTriggered = false;
            absorbPrepareFinished = false;
        }
    }
}
