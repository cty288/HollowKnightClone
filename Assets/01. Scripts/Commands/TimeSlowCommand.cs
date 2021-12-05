using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public class TimeSlowCommand : AbstractCommand<TimeSlowCommand> {
        private float slowTime;
        private float slowSpeed;
        protected override bool AutoRecycle { get; } = false;

        public static TimeSlowCommand Allocate(float slowTime, float slowSpeed) {
            TimeSlowCommand cmd = SafeObjectPool<TimeSlowCommand>.Singleton.Allocate();
            cmd.slowSpeed = slowSpeed;
            cmd.slowTime = slowTime;
            
            return cmd;
        }
        protected override void OnExecute() {
            Time.timeScale = slowSpeed;
            this.GetSystem<ITimeSystem>().AddDelayTask(slowSpeed * slowTime, () => {
                Time.timeScale = 1;
                RecycleToCache();
            });
        }
    }
}
