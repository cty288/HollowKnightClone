using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Examples.ServiceLocator;
using UnityEngine;
using FSM = MikroFramework.FSM.FSM;

namespace HollowKnight
{
    public enum EnemyName {
        Rat,
        Crow,
        ChargeMonster,
        FlyMonster,
        Boss
    }
    public interface IAbsorbable: IAttackable {
        public BindableProperty<bool> Absorbed { get; }
        public bool CanAbsorbWhenAlive { get; }
        public WeaponName WeaponName { get; }
        public void Absorb();

        public float HealthRestoreWhenAbsorb { get; }

        public bool HealthRestored { get; }

        public void Drop();
    }

    public interface IAttackable {
        public float MaxHealth { get; }

        public BindableProperty<float> Health { get; }

        public void Attack(float damage);

        public void Kill();

        void Restore();
    }

    public interface ICanAttack {
        public List<Enum> AttackStageNames { get; }
        public Dictionary<Enum, float> AttackSkillDamages { get; }
        public Dictionary<Enum, float> AttackFreqs { get; }
    }

    
    public abstract class EnemyConfigurationItem {
        public abstract EnemyName name { get; }

        public FSM FSM = new FSM();

        public EnemyConfigurationItem() {
            AddStateMachineState();
        }

        protected abstract void AddStateMachineState();
    }

    public abstract class AbstractAbsorbableConfiguration : EnemyConfigurationItem, IAbsorbable {
        public abstract float MaxHealth { get; }
        public abstract BindableProperty<float> Health { get; } 
        //will never surpass the existing health of that enemy
        public void Attack(float damage) {
            
            if (Health.Value > damage) {
                Health.Value -= damage;
            }
            else {
                Health.Value = 0;
            }
            
        }

        public void Kill() {
            Health.Value = 0;
        }

        public void Restore() {
            Health.Value = MaxHealth;
        }

        public BindableProperty<bool> Absorbed { get; } = new BindableProperty<bool>() { Value = false };
        public abstract bool CanAbsorbWhenAlive { get; }
        public abstract WeaponName WeaponName { get; }

        public void Absorb() {
            Absorbed.Value = true;
            HealthRestored = true;
        }

        public abstract float HealthRestoreWhenAbsorb { get; }
        public bool HealthRestored { get; set; } = false;

        public void Drop() {
            Absorbed.Value = false;
        }

      
    }

    public abstract class AbstractAbsorableCanAttackEnemyConfiguration : AbstractAbsorbableConfiguration, ICanAttack {
        public abstract List<Enum> AttackStageNames { get; }
        public abstract Dictionary<Enum, float> AttackSkillDamages { get; }
        public abstract Dictionary<Enum, float> AttackFreqs { get; }
        /// <summary>
        /// Raycast length
        /// </summary>
        public abstract float ViewDistance { get; }
    }

    public abstract class AbstractCanAttackEnemyConfiguration :EnemyConfigurationItem, ICanAttack, IAttackable {
        public abstract List<Enum> AttackStageNames { get; }
        public abstract Dictionary<Enum, float> AttackSkillDamages { get; }
        public abstract Dictionary<Enum, float> AttackFreqs { get; }

        public abstract float MaxHealth { get; }
        public abstract BindableProperty<float> Health { get; }

        public void Attack(float damage)
        {
            if (Health.Value > damage)
            {
                Health.Value -= damage;
            }
            else
            {
                Health.Value = 0;
            }

        }
        
        

        public void Kill()
        {
            Health.Value = 0;
        }

        public void Restore() {
            Health.Value = MaxHealth;
        }
    }
    public class RatConfiguration : AbstractAbsorbableConfiguration, IAbsorbable {
        public override float MaxHealth { get; } = 1;
        public override BindableProperty<float> Health { get; } = new BindableProperty<float>(){Value = 1};

        public override bool CanAbsorbWhenAlive { get; } = true;
        public override WeaponName WeaponName { get; } = WeaponName.Rat;
        public override float HealthRestoreWhenAbsorb { get; } = 0.5f;

        public override EnemyName name { get; } = EnemyName.Rat;

        protected override void AddStateMachineState() {
            FSM.AddTranslation(SmallAnimalState.Patrol, SmallAnimalEvents.Absorb, SmallAnimalState.Die, null)
                .Start(SmallAnimalState.Patrol);
        }
    }

    public class CrowConfiguration : AbstractAbsorbableConfiguration, IAbsorbable {
        public override EnemyName name { get; } = EnemyName.Crow;
        protected override void AddStateMachineState() {
            
        }

        public override float MaxHealth { get; } = 2;
        public override BindableProperty<float> Health { get; } = new BindableProperty<float>() {Value = 2};
        public override bool CanAbsorbWhenAlive { get; } = true;
        public override WeaponName WeaponName { get; } = WeaponName.Crow;
        public override float HealthRestoreWhenAbsorb { get; } = 1;
    }

    public class ChargeMonsterConfigurtion : AbstractAbsorableCanAttackEnemyConfiguration, IAbsorbable {
        public enum ChargeMonsterStages {
            Any,
            Idle,
            Patrolling,
            Chasing,
            Attacking,
            Dizzy,
            Die
        }

        public enum ChargeMonsterEvents {
            WaitEnds,
            WaitForChangeDirection,
            PlayerOutChagseRange,
            PlayerInChaseRange,
            PlayerInAttackRange,
            AttackDizzy,
            BeAttacked,
            Killed
        }
        public override EnemyName name { get; } = EnemyName.ChargeMonster;
        
        protected override void AddStateMachineState() {
            FSM
                .AddTranslation(ChargeMonsterStages.Idle, ChargeMonsterEvents.WaitEnds, ChargeMonsterStages.Patrolling,
                    null)
                .AddTranslation(ChargeMonsterStages.Patrolling, ChargeMonsterEvents.WaitForChangeDirection,
                    ChargeMonsterStages.Idle, null)
                .AddTranslation(ChargeMonsterStages.Dizzy, ChargeMonsterEvents.WaitEnds, ChargeMonsterStages.Patrolling,
                    null)
                .AddTranslation(ChargeMonsterStages.Any, ChargeMonsterEvents.PlayerInChaseRange,
                    ChargeMonsterStages.Chasing, null)
                .AddTranslation(ChargeMonsterStages.Any, ChargeMonsterEvents.PlayerInAttackRange,
                    ChargeMonsterStages.Attacking, null)
                .AddTranslation(ChargeMonsterStages.Attacking, ChargeMonsterEvents.AttackDizzy, ChargeMonsterStages.Dizzy, null)
                .AddTranslation(ChargeMonsterStages.Any, ChargeMonsterEvents.BeAttacked, ChargeMonsterStages.Dizzy, null).
                AddTranslation(ChargeMonsterStages.Any, ChargeMonsterEvents.PlayerOutChagseRange, ChargeMonsterStages.Patrolling, null).
                AddTranslation(ChargeMonsterStages.Any, ChargeMonsterEvents.Killed, ChargeMonsterStages.Die, null).
                Start(ChargeMonsterStages.Patrolling);
        }

        public override float MaxHealth { get; } = 10;
        public override BindableProperty<float> Health { get; } = new BindableProperty<float>() {Value = 10};

        public override bool CanAbsorbWhenAlive { get; } = false;
        public override WeaponName WeaponName { get; } = WeaponName.ChargeMonster;
        public override float HealthRestoreWhenAbsorb { get; } = 6;

        public override List<Enum> AttackStageNames { get; } = new List<Enum>() {ChargeMonsterStages.Attacking};




        public override Dictionary<Enum, float> AttackSkillDamages { get; } = new Dictionary<Enum, float>() {
            {ChargeMonsterStages.Attacking, 5}
        };
        public override Dictionary<Enum, float> AttackFreqs { get; } = new Dictionary<Enum, float>() {
            {ChargeMonsterStages.Attacking, 2f}
        };
        public override float ViewDistance { get; } = 6;
    }


    public class FlyMonsterConfiguration : AbstractAbsorableCanAttackEnemyConfiguration, IAbsorbable {
        public enum FlyMonsterStages
        {
            Any,
            Patrolling,
            Engaging,
            Die
        }

        public enum FlyMonsterEvents
        {
           PlayerOutRange,
           PlayerInRange,
           Killed
        }
        public override EnemyName name { get; } = EnemyName.FlyMonster;

        protected override void AddStateMachineState() {
            FSM.AddTranslation(FlyMonsterStages.Engaging, FlyMonsterEvents.PlayerOutRange, FlyMonsterStages.Patrolling, null).
                AddTranslation(FlyMonsterStages.Patrolling, FlyMonsterEvents.PlayerInRange, FlyMonsterStages.Engaging,null).
                AddTranslation(FlyMonsterStages.Any, FlyMonsterEvents.Killed, FlyMonsterStages.Die, null ).
                Start(FlyMonsterStages.Patrolling);
        }

        public override float MaxHealth { get; } = 6;
        public override BindableProperty<float> Health { get; } = new BindableProperty<float>() { Value = 6 };

        public override bool CanAbsorbWhenAlive { get; } = false;
        public override WeaponName WeaponName { get; } = WeaponName.FlyMonster;
        public override float HealthRestoreWhenAbsorb { get; } = 3;

        public override List<Enum> AttackStageNames { get; } = new List<Enum>() { FlyMonsterStages.Engaging };
        
        public override Dictionary<Enum, float> AttackSkillDamages { get; } = new Dictionary<Enum, float>() {
            {FlyMonsterStages.Engaging, 3}
        };
        public override Dictionary<Enum, float> AttackFreqs { get; } = new Dictionary<Enum, float>() {
            {FlyMonsterStages.Engaging, 2.5f}
        };
        public override float ViewDistance { get; } = 6;
    }

    public class BossConfiguration : AbstractCanAttackEnemyConfiguration {

        public enum BossStages
        {
            Any,
            Null,
            Dizzy,
            Attack,
            Shockwave,
            JumpAttack,
            LeftRightAttack,
            Walk,
            Die
        }

        public enum BossEvents
        {
            MovementFinish,
            MoveBack,
            Attack,
            Shockwave,
            JumpAttack,
            LeftRightAttack,
            Killed
        }

        public override List<Enum> AttackStageNames { get; } = new List<Enum>() {
            BossStages.Attack, BossStages.Shockwave, BossStages.JumpAttack, BossStages.LeftRightAttack
        };
        public override Dictionary<Enum, float> AttackSkillDamages { get; } = new Dictionary<Enum, float>() {
            {BossStages.Attack, 5},
            {BossStages.Shockwave, 8},
            {BossStages.JumpAttack, 10},
            {BossStages.LeftRightAttack, 5}
        };
        public override Dictionary<Enum, float> AttackFreqs { get; }
        public override float MaxHealth { get; } = 100;

        public override BindableProperty<float> Health { get; } = new BindableProperty<float>();
        public override EnemyName name { get; } = EnemyName.Boss;
        

        protected override void AddStateMachineState() {
            Health.Value = MaxHealth;
            FSM.
                AddTranslation(BossStages.Dizzy, BossEvents.Attack, BossStages.Attack,null).
                AddTranslation(BossStages.Dizzy,BossEvents.Shockwave, BossStages.Shockwave,null).
                AddTranslation(BossStages.Dizzy, BossEvents.JumpAttack, BossStages.JumpAttack,null).
                AddTranslation(BossStages.Dizzy,BossEvents.LeftRightAttack, BossStages.LeftRightAttack,null).
                AddTranslation(BossStages.Dizzy, BossEvents.MoveBack, BossStages.Walk,null).
                AddTranslation(BossStages.Any, BossEvents.Killed, BossStages.Die, null).
                AddTranslation(BossStages.Any, BossEvents.MovementFinish, BossStages.Dizzy, null).
                Start(BossStages.Dizzy);
        }
    }

}
