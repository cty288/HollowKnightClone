using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class HollowKnight : Architecture<HollowKnight> { 
        protected override void Init() {
            this.RegisterSystem<IMapSystem>(new MapSystem());
            this.RegisterSystem<IWeaponSystem>(new WeaponSystem());
            this.RegisterSystem<ITeleportSystem>(new TeleportSystem());

           
            this.RegisterModel<IPlayerConfigurationModel>(new PlayerConfigurationModel());
            this.RegisterModel<IPlayerModel>(new PlayerModel());
            this.RegisterModel<IWeaponTypeConfigModel>(new WeaponTypeConfigModel());
            this.RegisterModel<IWeaponConfigModel>(new WeaponConfigModel());

        }
    }
}
