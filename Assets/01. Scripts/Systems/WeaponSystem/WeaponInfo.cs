using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace HollowKnight
{

    public class WeaponInfo {
        public BindableProperty<WeaponName> Name = new BindableProperty<WeaponName>();
        public BindableProperty<WeaponType> Type = new BindableProperty<WeaponType>();

        public BindableProperty<ICommand> AttackSkill = new BindableProperty<ICommand>();
        public BindableProperty<int> AttackDamage = new BindableProperty<int>();
        public BindableProperty<float> AttackFreq = new BindableProperty<float>();

        public BindableProperty<float> ChargeAttackTime = new BindableProperty<float>();
        public BindableProperty<float> ChargeAttackDamage = new BindableProperty<float>();


        public BindableProperty<ICommand> Ult = new BindableProperty<ICommand>();
        public BindableProperty<float> UltChargeTime = new BindableProperty<float>() ;
        public BindableProperty<int> UltDamage= new BindableProperty<int>();

        public BindableProperty<int> BulletCount = new BindableProperty<int>();

        public WeaponInfo() { }
        public WeaponInfo(WeaponName name, WeaponType type, ICommand attackSkill,
            int attackDamage, float AttackFreq, float chargeAttackTime,
            float chargeAttackDamage, ICommand ult, float ultChargeTime,
            int ultDamage, int bulletCount) {
            this.Name.Value = name;
            this.Type.Value = type;
            this.AttackSkill.Value = attackSkill;
            this.AttackDamage.Value = attackDamage;
            this.AttackFreq.Value = AttackFreq;
            this.ChargeAttackTime.Value = chargeAttackTime;
            this.ChargeAttackDamage.Value = chargeAttackDamage;
            this.Ult.Value = ult;
            this.UltChargeTime.Value = ultChargeTime;
            this.UltDamage.Value = ultDamage;
            this.BulletCount.Value = bulletCount;
        }
    }

}
