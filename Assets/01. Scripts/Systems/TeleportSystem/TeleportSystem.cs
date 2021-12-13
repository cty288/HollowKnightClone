using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public enum TeleportState {
        NotTeleporting,
        PrepareTeleport,
        Teleporting,
        TeleportAppearing
    }
    public interface ITeleportSystem: ISystem {
        TeleportState TeleportState { get; }
        float MinTeleportDistance { get; }
        float MaxTeleportDistance { get; }
        void Teleport(Vector2 mousePosition);

        void OnReachDest(Vector2 arrowDest);
        float TeleportPrepareTime { get; }
        float TeleportFinishTime { get; }

        void Reset();
    }

    public struct OnTeleportPrepare {
        public Vector2 targetDest;
    }

    public struct OnTeleportStart {
        public Vector2 targetDest;
    }

    public struct OnTeleportInterrupted {
        
    }

    public struct OnTeleportAppearing {
        public Vector2 pos;
    }

    public struct OnTeleportFinished {

    }

    public class TeleportSystem : AbstractSystem, ITeleportSystem {
       
        private TimeSystem timer;
        
        protected override void OnInit() {
            
            timer = new TimeSystem();
            timer.Start();
          
        }

        public TeleportState TeleportState { get; set; } = TeleportState.NotTeleporting;
        public float MinTeleportDistance { get; } = 4;
        public float MaxTeleportDistance { get; } = 18;

        private DateTime teleportTime;

        public void Teleport(Vector2 mousePosition) {
            if (TeleportState == TeleportState.NotTeleporting) {
                Vector2 pos =  Camera.main.ScreenToWorldPoint(mousePosition);
               
                float dist = Mathf.Abs(Vector2.Distance(pos, Player.Singleton.transform.position));
                if (dist < MaxTeleportDistance && dist > MinTeleportDistance) {
                    //teleport success
                    TeleportState = TeleportState.PrepareTeleport;
                    this.SendEvent<OnTeleportPrepare>(new OnTeleportPrepare() { targetDest = pos });

                    timer.AddDelayTask(TeleportPrepareTime, () => {
                        this.SendEvent<OnTeleportStart>(new OnTeleportStart() { targetDest = pos });
                        TeleportState = TeleportState.Teleporting;
                        teleportTime = DateTime.Now;
                        Debug.Log("Teleporting");
                    });
                    
                }
            }else if (TeleportState == TeleportState.Teleporting) { //stop teleport
                if ((DateTime.Now - teleportTime).TotalMilliseconds >= 100) {
                    this.SendEvent<OnTeleportInterrupted>();
                }
              

                
            }
        }

        public void OnReachDest(Vector2 arrowDest) {
            TeleportState = TeleportState.TeleportAppearing;
            this.SendEvent<OnTeleportAppearing>(new OnTeleportAppearing(){pos = arrowDest});

            timer.AddDelayTask(TeleportFinishTime, () => {
                this.SendEvent<OnTeleportFinished>();
                TeleportState = TeleportState.NotTeleporting;
            });
        }

        public float TeleportPrepareTime { get; } = 0.4f;
        public float TeleportFinishTime { get; } = 0.7f;
        public void Reset() {
            TeleportState = TeleportState.NotTeleporting;
        }
    }
}
