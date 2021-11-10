using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IMapSystem : ISystem {

    }

    public class MapSystem : AbstractSystem, IMapSystem
    {
        protected override void OnInit() {
            
        }
    }
}
