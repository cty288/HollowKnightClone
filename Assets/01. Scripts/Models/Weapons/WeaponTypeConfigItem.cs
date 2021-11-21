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
        public ICommand AttackSkill;
        public int AttackDamage;
        public float AttackFreq;

        public float ChargeAttackTime;
        public float ChargeAttackDamage;


        public ICommand Ult;
        public float UltChargeTime;
        public int UltDamage;

        
        public WeaponTypeConfigItem(WeaponType type, ICommand attackSkill, 
            int skillDamage, float skillFreq, float chargeAttackTime,
            float chargeAttackDamage, ICommand ult, float ultChargeTime, int ultDamage) {
            this.Type = type;
            this.AttackSkill = attackSkill;
            this.AttackDamage = skillDamage;
            this.AttackFreq = skillFreq;
            this.ChargeAttackDamage = chargeAttackDamage;
            this.ChargeAttackTime = chargeAttackTime;
            this.Ult = ult;
            this.UltChargeTime = ultChargeTime;
            this.UltDamage = ultDamage;
        }
    }
}
