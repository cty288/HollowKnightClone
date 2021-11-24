using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class ShakeCameraCommand : AbstractCommand<ShakeCameraCommand>
    {
        private float duration;
        private float strength = 3;
        private int vibrato = 10;
        private float randomness = 90;
        public static ShakeCameraCommand Allocate(float duration, float strength=3f, int vibrato=10, float randomness=90) {
            ShakeCameraCommand cmd = SafeObjectPool<ShakeCameraCommand>.Singleton.Allocate();
            cmd.duration = duration;
            cmd.strength = strength;
            cmd.vibrato = vibrato;
            cmd.randomness = randomness;
            return cmd;
        }

        protected override void OnExecute() {
            this.SendEvent<ShakeCameraEvent>(new ShakeCameraEvent() {
                Duration = duration,
                Randomness = randomness,
                Vibrato = vibrato,
                Strength = strength
            });
        }
    }
}
