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

        float TeleportPrepareTime { get; }
        float TeleportFinishTime { get; }
    }

    public struct OnTeleportPrepare {
        public Vector2 targetDest;
    }

    public struct OnTeleportStart {
        public Vector2 targetDest;
    }

    public struct OnTeleportInterrupted {
        
    }

    public class TeleportSystem : AbstractSystem, ITeleportSystem {
        private Player player;
        private TimeSystem timer;
        private Camera cam;
        protected override void OnInit() {
            player = Player.Singleton;
            timer = new TimeSystem();
            timer.Start();
            cam = Camera.main;
        }

        public TeleportState TeleportState { get; set; } = TeleportState.NotTeleporting;
        public float MinTeleportDistance { get; } = 4;
        public float MaxTeleportDistance { get; } = 18;

        public void Teleport(Vector2 mousePosition) {
            if (TeleportState == TeleportState.NotTeleporting) {
                Vector2 pos =  cam.ScreenToWorldPoint(mousePosition);
                Debug.Log(pos);
                float dist = Mathf.Abs(Vector2.Distance(pos, player.transform.position));
                if (dist < MaxTeleportDistance && dist > MinTeleportDistance) {
                    //teleport success
                    TeleportState = TeleportState.PrepareTeleport;
                    this.SendEvent<OnTeleportPrepare>(new OnTeleportPrepare() { targetDest = pos });

                    timer.AddDelayTask(TeleportPrepareTime, () => {
                        this.SendEvent<OnTeleportStart>(new OnTeleportStart() { targetDest = pos });
                        TeleportState = TeleportState.Teleporting;
                        Debug.Log("Teleporting");
                    });
                    
                }
            }else if (TeleportState == TeleportState.Teleporting) { //stop teleport
                this.SendEvent<OnTeleportInterrupted>();
            }
        }

        public float TeleportPrepareTime { get; } = 0.28f;
        public float TeleportFinishTime { get; } = 0.5f;
    }
}
