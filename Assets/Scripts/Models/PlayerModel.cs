using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace HollowKnight
{
    public interface IPlayerModel : IModel {
        BindableProperty<float> MovementAcceleration { get; }
        BindableProperty<float> MaxMoveSpeed { get; }

        BindableProperty<float> GroundLinearDrag { get; }

        BindableProperty<float> AirLinearDrag { get; }

        BindableProperty<float> JumpForce { get; }
        BindableProperty<float> FallMultiplier { get; }
        BindableProperty<float> LowFallMultiplier { get; }
        BindableProperty<int> RemainingExtraJump { get; }

        BindableProperty<float> DashSpeed { get; }

        BindableProperty<float> GroundRaycastLength { get; }

        void ChangeRemainingJumpValue(int value);

        void ResetRemainingJumpValue();

    }
    public class PlayerModel : AbstractModel, IPlayerModel
    {
        public BindableProperty<float> MovementAcceleration { get; } = new BindableProperty<float>();
        public BindableProperty<float> MaxMoveSpeed { get; } = new BindableProperty<float>();
        public BindableProperty<float> GroundLinearDrag { get; } = new BindableProperty<float>();
        public BindableProperty<float> AirLinearDrag { get; } = new BindableProperty<float>();
        public BindableProperty<float> JumpForce { get; } = new BindableProperty<float>();
        public BindableProperty<float> FallMultiplier { get; } = new BindableProperty<float>();
        public BindableProperty<float> LowFallMultiplier { get; } = new BindableProperty<float>();
        public BindableProperty<int> RemainingExtraJump { get; } = new BindableProperty<int>();
        public BindableProperty<float> DashSpeed { get; } = new BindableProperty<float>();
        public BindableProperty<float> GroundRaycastLength { get; } = new BindableProperty<float>();
        public void ChangeRemainingJumpValue(int value) {
            RemainingExtraJump.Value += value;
        }

        public void ResetRemainingJumpValue() {
            RemainingExtraJump.Value = this.GetModel<IPlayerConfigurationModel>().ExtraJumps;
        }

        protected override void OnInit() {
            IPlayerConfigurationModel playerConfigurationModel = this.GetModel<IPlayerConfigurationModel>();
            MovementAcceleration.Value = playerConfigurationModel.MovementAcceleration;
            GroundLinearDrag.Value = playerConfigurationModel.GroundLinearDrag;
            AirLinearDrag.Value = playerConfigurationModel.AirLinearDrag;
            JumpForce.Value = playerConfigurationModel.JumpForce;
            FallMultiplier.Value = playerConfigurationModel.FallMultiplier;
            LowFallMultiplier.Value = playerConfigurationModel.LowFallMultiplier;
            RemainingExtraJump.Value = playerConfigurationModel.ExtraJumps;
            DashSpeed.Value = playerConfigurationModel.DashSpeed;
            GroundRaycastLength.Value = playerConfigurationModel.GroundRaycastLength;
        }

       
        
    }
}
