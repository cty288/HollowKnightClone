using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IAbsorbSystem : ISystem {

    }
    public class AbsorbSystem : AbstractSystem, IAbsorbSystem {
        protected override void OnInit() {
            
        }
    }
}
