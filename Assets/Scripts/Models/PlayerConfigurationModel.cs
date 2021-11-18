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
        float AirLinearDrag { get; }
        float JumpForce { get; }
        float FallMultiplier { get; }
        float LowFallMultiplier { get; }
        int ExtraJumps { get; }

        float DashSpeed { get; }

        float GroundRaycastLength { get; }
    }

    public class PlayerConfigurationModel : AbstractModel, IPlayerConfigurationModel
    {
        protected override void OnInit() {
            
        }

        public float MovementAcceleration { get; } = 42;
        public float MaxMoveSpeed { get; } = 15;
        public float GroundLinearDrag { get; } = 18;
        public float AirLinearDrag { get; } = 2.5f; 
        public float JumpForce { get; } = 25;
        public float FallMultiplier { get; } = 8;
        public float LowFallMultiplier { get; } = 5;
        public int ExtraJumps { get; } = 2;
        public float DashSpeed { get; } = 0;
        public float GroundRaycastLength { get; } = 0.7f;
    }
}
