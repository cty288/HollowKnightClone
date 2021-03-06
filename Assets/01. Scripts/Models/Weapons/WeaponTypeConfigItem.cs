using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace HollowKnight
{
    public enum WeaponType {
        SmallAnimal,
        Humanoid
    }
    public class WeaponTypeConfigItem
    {
        public WeaponType Type;
        public IWeaponCommand AttackSkill;
        public int AttackDamage;
        public float AttackFreq;

        public IWeaponCommand ChargeAttackSkill;
        public float ChargeAttackTime;
        public float ChargeAttackDamage;


        public IWeaponCommand Ult;
        public float UltChargeTime;
        public int UltDamage;
        public bool UltNeedTarget;

        public bool NeedTargetWhenAttack;
        
        public WeaponTypeConfigItem(WeaponType type, IWeaponCommand attackSkill,
            int skillDamage, float skillFreq, IWeaponCommand chargeAttackSkill, float chargeAttackTime,
            float chargeAttackDamage, bool needTargetWhenAttack, IWeaponCommand ult, float ultChargeTime, int ultDamage, bool ultNeedTarget) {
            this.Type = type;
            this.ChargeAttackSkill = chargeAttackSkill;
            this.AttackSkill = attackSkill;
            this.AttackDamage = skillDamage;
            this.AttackFreq = skillFreq;
            this.ChargeAttackDamage = chargeAttackDamage;
            this.ChargeAttackTime = chargeAttackTime;
            this.NeedTargetWhenAttack = needTargetWhenAttack;
            this.Ult = ult;
            this.UltChargeTime = ultChargeTime;
            this.UltDamage = ultDamage;
            this.UltNeedTarget = ultNeedTarget;
        }
    }
}
