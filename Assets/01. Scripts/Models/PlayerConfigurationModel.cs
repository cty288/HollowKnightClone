using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IPlayerConfigurationModel : IModel {
        float MovementAcceleration { get; }
        float MaxMoveSpeed { get; }

        float GroundLinearDrag { get; } 
      
        float JumpForce { get; }
   
        int ExtraJumps { get; }

        float DashSpeed { get; }


    }

    public class PlayerConfigurationModel : AbstractModel, IPlayerConfigurationModel
    {
        protected override void OnInit() {
            
        }

        public float MovementAcceleration { get; } = 15;
        public float MaxMoveSpeed { get; } = 6;
        public float GroundLinearDrag { get; } = 0.9f;
       
        public float JumpForce { get; } = 15;
        
        public int ExtraJumps { get; } = 1;
        public float DashSpeed { get; } = 0;
        
    }
}
