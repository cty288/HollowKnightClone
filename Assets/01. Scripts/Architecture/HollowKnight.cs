using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public class HollowKnight : Architecture<HollowKnight> { 
        protected override void Init() {
            this.RegisterEvent<OnPlayerRespawned>(Reset);
            this.RegisterEvent<OnGameEnd>(Reset);

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

        public void Reset(OnPlayerRespawned e) {
            Debug.Log("All Reset");
            this.GetSystem<IWeaponSystem>().Reset();
            this.GetSystem<ITeleportSystem>().Reset();
            this.GetSystem<IAbsorbSystem>().Reset();
            this.GetSystem<IAttackSystem>().Reset();
            this.GetSystem<ITimeSystem>().Reset();
            this.GetSystem<IBuffSystem>().Reset();

            this.GetModel<IPlayerModel>().Reset();
        }

        public void Reset(OnGameEnd e)
        {
            Debug.Log("All Reset");
            this.RegisterSystem<IWeaponSystem>(new WeaponSystem());
            this.RegisterSystem<ITeleportSystem>(new TeleportSystem());
            this.RegisterSystem<IAbsorbSystem>(new AbsorbSystem());
            this.RegisterSystem<IAttackSystem>(new AttackSystem());
            this.RegisterSystem<ITimeSystem>(new TimeSystem());
            this.RegisterSystem<IBuffSystem>(new BuffSystem());

            this.RegisterModel<IPlayerModel>(new PlayerModel());
        }
    }
}
