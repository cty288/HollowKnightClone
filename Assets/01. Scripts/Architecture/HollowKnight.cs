using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public class HollowKnight : Architecture<HollowKnight> { 
        protected override void Init() {
          //  this.RegisterSystem<IMapSystem>(new MapSystem());
            this.RegisterSystem<IWeaponSystem>(new WeaponSystem());
            this.RegisterSystem<ITeleportSystem>(new TeleportSystem());
            this.RegisterSystem<IAbsorbSystem>(new AbsorbSystem());
            this.RegisterSystem<IAttackSystem>(new AttackSystem());
            this.RegisterSystem<ITimeSystem> (new TimeSystem());
            this.RegisterSystem<IBuffSystem>(new BuffSystem());
           
            this.RegisterModel<IPlayerConfigurationModel>(new PlayerConfigurationModel());
            this.RegisterModel<IPlayerModel>(new PlayerModel());
            this.RegisterModel<IWeaponTypeConfigModel>(new WeaponTypeConfigModel());
            this.RegisterModel<IWeaponConfigModel>(new WeaponConfigModel());
            this.RegisterModel<IEnemyConfigurationModel>(new EnemyConfigurationModel());
        }
    }
}
