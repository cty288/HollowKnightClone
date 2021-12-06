using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public struct OnBuffTimeUp {
        public BuffType BuffType;
    }

    public struct OnBuffStart {
        public BuffType BuffType;
    }
    public enum BuffType {
        SmallAnimalUnlimitedBullet,
        HumanoidNormalAttackFaster
    }

    public class Buff {
        public BuffType BuffType;
        public float RemainingTime;
    }

    public interface IBuffSystem : ISystem {
        void AddBuff(BuffType buffType, float length);
        bool HasBuff(BuffType buffType);

        public float MinUpdateTimeInterval { get; }

        public float GetRemainingTime(BuffType buffType);

    }
    public class BuffSystem : AbstractSystem, IBuffSystem {
        private List<Buff> ongoingBuffs = new List<Buff>();

        private ITimeSystem timeSystem;

        protected override void OnInit() {
            timeSystem = this.GetSystem<ITimeSystem>();
            timeSystem.AddDelayTask(MinUpdateTimeInterval, UpdateBuffs);
        }

        public void AddBuff(BuffType buffType, float length) {
            Buff buff = GetBuff(buffType);

            if (buff!=null) {
                buff.RemainingTime += length;
            }
            else {
                ongoingBuffs.Add(new Buff(){BuffType = buffType, RemainingTime = length});
                this.SendEvent<OnBuffStart>(new OnBuffStart(){BuffType = buffType});
            }
        }

        

        public bool HasBuff(BuffType buffType) {
            return GetBuff(buffType) != null;
        }

        public float MinUpdateTimeInterval { get; } = 0.1f;
        public float GetRemainingTime(BuffType buffType) {
            Buff buff = GetBuff(buffType);

            if (buff!=null) {
                return buff.RemainingTime;
            }

            return 0;
        }

        private Buff GetBuff(BuffType type) {
            foreach (Buff buff in ongoingBuffs) {
                if (buff.BuffType == type) {
                    return buff;
                }
            }

            return null;
        }

        private void UpdateBuffs() {
            List<Buff> deletedBuffs = new List<Buff>();

            foreach (Buff buff in ongoingBuffs) {
                buff.RemainingTime -= MinUpdateTimeInterval;

                if (buff.RemainingTime <= 0) {
                    buff.RemainingTime = 0;
                    Debug.Log($"{buff.BuffType} times up");
                    this.SendEvent<OnBuffTimeUp>(new OnBuffTimeUp() { BuffType = buff.BuffType });
                    deletedBuffs.Add(buff);
                }
            }

            foreach (Buff deletedBuff in deletedBuffs) {
                ongoingBuffs.Remove(deletedBuff);
            }

            timeSystem.AddDelayTask(MinUpdateTimeInterval, UpdateBuffs);
        }
    }
}
