using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public enum WeaponName {
        Rat
    }
    public class WeaponConfigItem {
        public WeaponTypeConfigItem TypeConfigItem;
        public WeaponName WeaponName;
        public int WeaponCapacity;

        public WeaponConfigItem(WeaponName name,
            WeaponTypeConfigItem typeConfigItem, int capacity) {
            this.TypeConfigItem = typeConfigItem;
            this.WeaponName = name;
            this.WeaponCapacity = capacity;
        }
    }
}
