using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class HollowKnight : Architecture<HollowKnight> { 
        protected override void Init() {
            this.RegisterSystem<IMapSystem>(new MapSystem());
            this.RegisterModel<IPlayerConfigurationModel>(new PlayerConfigurationModel());
            this.RegisterModel<IPlayerModel>(new PlayerModel());
        }
    }
}
