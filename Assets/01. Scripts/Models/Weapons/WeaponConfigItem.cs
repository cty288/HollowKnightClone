using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public enum WeaponName {
        Rat,
        Crow,
        ChargeMonster,
        FlyMonster
    }
    public class WeaponConfigItem {
        public WeaponTypeConfigItem TypeConfigItem;
        public WeaponName WeaponName;
        public int WeaponCapacity;
        public int UltChargeAmountWhenDie;

        public WeaponConfigItem(WeaponName name,
            WeaponTypeConfigItem typeConfigItem, int capacity) {
            this.TypeConfigItem = typeConfigItem;
            this.WeaponName = name;
            this.WeaponCapacity = capacity;
        }
    }
}
