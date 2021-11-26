using System;
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

        public BindableProperty<IWeaponCommand> AttackSkill = new BindableProperty<IWeaponCommand>();
        public BindableProperty<int> AttackDamage = new BindableProperty<int>();
        public BindableProperty<float> AttackFreq = new BindableProperty<float>();

        public BindableProperty<IWeaponCommand> ChargeAttackSkill = new BindableProperty<IWeaponCommand>();
        public BindableProperty<float> ChargeAttackTime = new BindableProperty<float>();
        public BindableProperty<float> ChargeAttackDamage = new BindableProperty<float>();


        public BindableProperty<IWeaponCommand> Ult = new BindableProperty<IWeaponCommand>();
        public BindableProperty<float> UltChargeTime = new BindableProperty<float>() ;
        public BindableProperty<int> UltDamage= new BindableProperty<int>();

        public BindableProperty<int> BulletCount = new BindableProperty<int>();

        public Action<WeaponInfo,int,int> OnBulletCountChange;

        public int MaxBulletCount;

        public WeaponInfo() { }
        public WeaponInfo(WeaponName name, WeaponType type, IWeaponCommand attackSkill,
            int attackDamage, float AttackFreq, IWeaponCommand chargeAttackSkill, float chargeAttackTime,
            float chargeAttackDamage, IWeaponCommand ult, float ultChargeTime,
            int ultDamage, int bulletCount, int MaxBulletCount) {

            this.Name.Value = name;
            this.Type.Value = type;
            this.AttackSkill.Value = attackSkill;
            this.AttackDamage.Value = attackDamage;
            this.ChargeAttackSkill.Value = chargeAttackSkill;
            this.AttackFreq.Value = AttackFreq;
            this.ChargeAttackTime.Value = chargeAttackTime;
            this.ChargeAttackDamage.Value = chargeAttackDamage;
            this.Ult.Value = ult;
            this.UltChargeTime.Value = ultChargeTime;
            this.UltDamage.Value = ultDamage;
            this.BulletCount.Value = bulletCount;
            this.MaxBulletCount = MaxBulletCount;
            
            BulletCount.RegisterOnValueChaned(OnBulletChange);
        }

        private void OnBulletChange(int old, int newBullet) {
            OnBulletCountChange?.Invoke(this,old,newBullet);
        }

        
    }

}
