using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace HollowKnight
{
    public struct OnUltChargeToMax {

    }
    public interface IPlayerModel : IModel {
      
        BindableProperty<float> WalkSpeed { get; }
        BindableProperty<float> RunSpeed { get; }

        BindableProperty<float> MaxWalkSpeed { get; }
        BindableProperty<float> MaxRunSpeed { get; }

        BindableProperty<float> GroundLinearDrag { get; }

        BindableProperty<float> AirLinearDrag { get; }

        BindableProperty<float> JumpForce { get; }
        BindableProperty<float> FallMultiplier { get; }
        BindableProperty<float> LowFallMultiplier { get; }
        BindableProperty<int> RemainingExtraJump { get; }

        BindableProperty<float> DashSpeed { get; }

        BindableProperty<float> GroundRaycastLength { get; }

        BindableProperty<int> UltChargeAccumlated { get; }

        void ChangeRemainingJumpValue(int value);

        void ResetRemainingJumpValue();

        void AddUlt(int amount);
    }
    public class PlayerModel : AbstractModel, IPlayerModel
    {
       
        public BindableProperty<float> WalkSpeed { get; } = new BindableProperty<float>();
        public BindableProperty<float> RunSpeed { get; } = new BindableProperty<float>();

        public BindableProperty<float> MaxWalkSpeed { get; } = new BindableProperty<float>();
        public BindableProperty<float> MaxRunSpeed { get; } = new BindableProperty<float>();
        public BindableProperty<float> GroundLinearDrag { get; } = new BindableProperty<float>();
        public BindableProperty<float> AirLinearDrag { get; } = new BindableProperty<float>();
        public BindableProperty<float> JumpForce { get; } = new BindableProperty<float>();
        public BindableProperty<float> FallMultiplier { get; } = new BindableProperty<float>();
        public BindableProperty<float> LowFallMultiplier { get; } = new BindableProperty<float>();
        public BindableProperty<int> RemainingExtraJump { get; } = new BindableProperty<int>();
        public BindableProperty<float> DashSpeed { get; } = new BindableProperty<float>();
        public BindableProperty<float> GroundRaycastLength { get; } = new BindableProperty<float>();
        public BindableProperty<int> UltChargeAccumlated { get; } = new BindableProperty<int>();

        public void ChangeRemainingJumpValue(int value) {
            RemainingExtraJump.Value += value;
        }

        public void ResetRemainingJumpValue() {
            RemainingExtraJump.Value = this.GetModel<IPlayerConfigurationModel>().ExtraJumps;
        }

        protected override void OnInit() {
            IPlayerConfigurationModel playerConfigurationModel = this.GetModel<IPlayerConfigurationModel>();
            WalkSpeed.Value = playerConfigurationModel.WalkSpeed;
            RunSpeed.Value = playerConfigurationModel.RunSpeed;

            GroundLinearDrag.Value = playerConfigurationModel.GroundLinearDrag;
           
            MaxWalkSpeed.Value = playerConfigurationModel.MaxWalkSpeed;
            MaxRunSpeed.Value = playerConfigurationModel.MaxRunSpeed;

            JumpForce.Value = playerConfigurationModel.JumpForce;
         
            RemainingExtraJump.Value = playerConfigurationModel.ExtraJumps;
            DashSpeed.Value = playerConfigurationModel.DashSpeed;

            UltChargeAccumlated.Value = 0;
        }


        public void AddUlt(int amount) {
            IPlayerConfigurationModel configuration = this.GetModel<IPlayerConfigurationModel>();
            if (configuration.MaxUltChargeNeeded - UltChargeAccumlated.Value >= amount) {
                UltChargeAccumlated.Value += amount;
            }
            else {
                UltChargeAccumlated.Value = configuration.MaxUltChargeNeeded;
                //charge to max
                this.SendEvent<OnUltChargeToMax>(new OnUltChargeToMax());
            }
           
        }
    }
}
