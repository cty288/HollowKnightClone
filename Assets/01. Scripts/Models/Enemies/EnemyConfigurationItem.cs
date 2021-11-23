using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples.ServiceLocator;
using UnityEngine;
using FSM = MikroFramework.FSM.FSM;

namespace HollowKnight
{
    public enum EnemyName {
        Rat
    }
    public interface IAbsorbable: IAttackable {
        public bool CanAbsorbWhenAlive { get; }
    }

    public interface IAttackable {
        public int Health { get; }
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
        public int Health { get; } = 1;
        public bool CanAbsorbWhenAlive { get; } = true;
        public override EnemyName name { get; } = EnemyName.Rat;

        protected override void AddStateMachineState() {
            FSM.AddTranslation(SmallAnimalState.Patrol, SmallAnimalEvents.Absorb, SmallAnimalState.Die, null)
                .Start(SmallAnimalState.Patrol);
        }
    }
}
