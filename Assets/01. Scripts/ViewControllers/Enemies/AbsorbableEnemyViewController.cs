using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class AbsorbableEnemyViewController : AbstractMikroController<HollowKnight>
    {
        /// <summary>
        /// This is only effective if this enemy is absorbable in the configuration model
        /// </summary>
        protected virtual void OnAbsorbing(float percentage) { }
        /// <summary>
        /// This is only effective if this enemy is absorbable in the configuration model
        /// </summary>
        protected virtual void OnAbsorbed() { }
    }
}
