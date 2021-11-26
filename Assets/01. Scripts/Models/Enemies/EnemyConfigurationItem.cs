using System.Collections;
using System.Collections.Generic;
using MikroFramework.BindableProperty;
using MikroFramework.Examples.ServiceLocator;
using UnityEngine;
using FSM = MikroFramework.FSM.FSM;

namespace HollowKnight
{
    public enum EnemyName {
        Rat
    }
    public interface IAbsorbable: IAttackable {
        public BindableProperty<bool> Absorbed { get; }
        public bool CanAbsorbWhenAlive { get; }
        public WeaponName WeaponName { get; }
        public void Absorb();

        public void Drop();
    }

    public interface IAttackable {
        public BindableProperty<int> Health { get; }

        public void Attack(int damage);

        public void Kill();
    }

    public interface ICanAttack {
    }

    
    public abstract class EnemyConfigurationItem {
        public abstract EnemyName name { get; }

        public FSM FSM = new FSM();

        public EnemyConfigurationItem() {
            AddStateMachineState();
        }

        protected abstract void AddStateMachineState();
    }

    public class RatConfiguration : EnemyConfigurationItem, IAbsorbable {
        public BindableProperty<int> Health { get; } = new BindableProperty<int>(){Value = 1};
        public void Attack(int damage) {
            Health.Value -= damage;
        }

        public void Kill() {
            Health.Value = 0;
        }

        public BindableProperty<bool> Absorbed { get; } = new BindableProperty<bool>() {Value = false};
        public bool CanAbsorbWhenAlive { get; } = true;
        public WeaponName WeaponName { get; } = WeaponName.Rat;

        public void Absorb() {
            Absorbed.Value = true;
        }

        public void Drop() {
            Absorbed.Value = false;
        }

        public override EnemyName name { get; } = EnemyName.Rat;

        protected override void AddStateMachineState() {
            FSM.AddTranslation(SmallAnimalState.Patrol, SmallAnimalEvents.Absorb, SmallAnimalState.Die, null)
                .Start(SmallAnimalState.Patrol);
        }
    }
}
