using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IPlayerConfigurationModel : IModel {
       
        float MaxWalkSpeed { get; }
        float MaxRunSpeed { get; }
        float WalkSpeed { get; }
        float RunSpeed { get; }
        float GroundLinearDrag { get; } 
      
        float JumpForce { get; }
   
        int ExtraJumps { get; }

        float DashSpeed { get; }

        int MaxUltChargeNeeded { get; }
        public float MaxHealth { get; }
    }

    public class PlayerConfigurationModel : AbstractModel, IPlayerConfigurationModel
    {
        protected override void OnInit() {
            
        }

       
        public float WalkSpeed { get; } = 8;
        public float RunSpeed { get; } = 14;
        public float MaxWalkSpeed { get; } = 4;
        public float MaxRunSpeed { get; } = 8;
        public float GroundLinearDrag { get; } = 0.9f;
       
        public float JumpForce { get; } = 15;
        
        public int ExtraJumps { get; } = 1;
        public float DashSpeed { get; } = 0;
        public int MaxUltChargeNeeded { get; } = 30;

        public float MaxHealth { get; } = 1;
    }
}
