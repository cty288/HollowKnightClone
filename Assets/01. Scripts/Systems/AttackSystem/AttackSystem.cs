using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public enum AttackState
    {
        NotAttacking,
        Preparing,
        Attacking
    }

    public struct OnAttackStop {

    }

    public struct OnAttackStartPrepare {

    }

    public struct OnAttackAiming {
        public GameObject Target;
    }
    public struct OnNormalAttack {
        public float TimeSinceLastNormalAttack;
        public IEnemyViewControllerAttackable AttackableViewController;
        public GameObject TargetGameObject;
    }

    public struct OnChargeAttackCharging {
        public float ChargeTime;
        public IEnemyViewControllerAttackable AttackableViewController;
        public GameObject TargetGameObject;
    }

    public struct OnChargeAttackRelease {
        public float TotalChargeTime;
        public IEnemyViewControllerAttackable AttackableViewController;
        public GameObject TargetGameObject;
    }

    public interface IAttackSystem : ISystem {
        public float AttackStopThreshold { get; }

        public AttackState AttackState { get; }

        public float ChargeAttackThreshold { get; }

        public float AttackPrepareTime { get; }

        public void StopAttack();

        public void CheckAttackPerFrame(PlayerState currentState);
    }
    public class AttackSystem : AbstractSystem, IAttackSystem {
        private AttackState attackState;
        private float attackTimer = 0;
        private float attackStopTimer = 0;

        private float normalAttackInterval = 0;

        private IWeaponSystem weaponSystem;

        private IEnemyViewControllerAttackable targetAttackable;
        protected override void OnInit() {
            attackState = AttackState.NotAttacking;
            weaponSystem = this.GetSystem<IWeaponSystem>();
        }

        public float AttackStopThreshold { get; } = 1f;
        public AttackState AttackState {
            get {
                return attackState;
            }
        }
        public float ChargeAttackThreshold { get; } = 0.2f;
        public float AttackPrepareTime { get; } = 0.5f;

        private bool isUltPreparing = false;
        public void StopAttack() {
            if (AttackState != AttackState.NotAttacking) {
                this.SendEvent<OnAttackStop>();
            }
            
            attackState = AttackState.NotAttacking;
            attackStopTimer = 0;
            targetAttackable = null;
            attackTimer = 0;
            isUltPreparing = false;
            normalAttackInterval = 0;
        }

        public void CheckAttackPerFrame(PlayerState currentState) {
            if (weaponSystem.SelectedWeapon != null) {
                if (attackState == AttackState.Attacking)
                {
                    attackStopTimer += Time.deltaTime;
                    if (attackStopTimer >= AttackStopThreshold)
                    {
                        StopAttack();
                    }
                }

                if (attackState == AttackState.NotAttacking && currentState == PlayerState.Normal)
                {
                    if (Input.GetMouseButtonDown(0) && DetectAttackTarget(out targetAttackable)) //select enemy
                    {
                        attackState = AttackState.Preparing;
                        this.SendEvent<OnAttackStartPrepare>();
                    }else if (CheckUlt()) {
                        DetectAttackTarget(out targetAttackable);
                        isUltPreparing = true;
                        attackState = AttackState.Preparing;
                        this.SendEvent<OnAttackStartPrepare>();
                    }

                }


                if (attackState == AttackState.Preparing || attackState == AttackState.Attacking) {
                    normalAttackInterval += Time.deltaTime;

                    if (attackState == AttackState.Preparing)
                    {
                        attackTimer += Time.deltaTime;
                        if (attackTimer >= AttackPrepareTime)
                        {
                            attackTimer = 0;
                            attackState = AttackState.Attacking;

                            if (isUltPreparing) {
                                Ult(targetAttackable);
                                StopAttack();
                                return;
                            }

                            if (!Input.GetMouseButton(0)) {
                                Debug.Log("Normal Attack");
                                NormalAttack();
                            }
                        }
                    }

                    if (attackState == AttackState.Attacking)
                    {
                        //select enemy
                        if (Input.GetMouseButtonDown(0)) {
                            DetectAttackTarget(out targetAttackable);
                        }

                        if (CheckUlt()) {
                            Ult(targetAttackable);
                            StopAttack();
                            return;
                        }

                        if (Input.GetMouseButton(0) && targetAttackable!=null)
                        { //charge
                            attackStopTimer = 0;
                            attackTimer += Time.deltaTime;
                            
                            this.SendEvent<OnAttackAiming>(new OnAttackAiming() {
                                Target = targetAttackable.GameObject
                            });
                            if (attackTimer > ChargeAttackThreshold)
                            {
                                ChargeAttackCharging(attackTimer);
                                /*
                                if (attackTimer > weaponSystem.SelectedWeapon.ChargeAttackTime.Value +
                                    ChargeAttackThreshold) { //reach maximum charge time
                                    ChargeAttackRelease(attackTimer);
                                    StopAttack();
                                    Debug.Log("Charge attack reaches maximum time");
                                }*/
                                
                            }


                        }

                        if (Input.GetMouseButtonUp(0) && targetAttackable !=null)
                        {
                            if (attackTimer <= ChargeAttackThreshold)
                            {
                                //normal attack
                                NormalAttack();
                                Debug.Log("Normal Attack");
                            }
                            else
                            {
                                //charge attack
                                ChargeAttackRelease(attackTimer);
                                Debug.Log("Charge Attack Finished");
                            }

                            attackTimer = 0;
                        }
                    }
                }
            }
            else {
                StopAttack();
            }
            
        }
        private LayerMask Mask = LayerMask.GetMask("Enemy", "EnemyTraversable");

        private bool DetectAttackTarget(out IEnemyViewControllerAttackable target) {
            Camera cam = Camera.main;

            RaycastHit2D ray = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Input.mousePosition), 1000, Mask);

            Collider2D collider = ray.collider;

            IEnemyViewControllerAttackable component = null;

            if (collider != null && collider.TryGetComponent<IEnemyViewControllerAttackable>(out component)) {
                if (!component.IsDie) {
                    target = component;
                    return true;
                }
            }

            target = null;
            return false;
        }

        private void Ult(IEnemyViewControllerAttackable attackable) {
            if (attackable != null) {
                Debug.Log($"Ult to {attackable.GameObject.name}");
            }
            else {
                Debug.Log($"Ult with no target");
            }
        }

        private void NormalAttack() {
            OnNormalAttack e = new OnNormalAttack(){TimeSinceLastNormalAttack = normalAttackInterval};
            if (targetAttackable != null) {
                e.AttackableViewController = targetAttackable;
                e.TargetGameObject = targetAttackable.GameObject;
            }

            this.SendEvent<OnNormalAttack>(e);
            normalAttackInterval = 0;
        }

        private void ChargeAttackCharging(float chargeTime) {
            OnChargeAttackCharging e = new OnChargeAttackCharging() { ChargeTime = chargeTime};
            if (targetAttackable != null)
            {
                e.AttackableViewController = targetAttackable;
                e.TargetGameObject = targetAttackable.GameObject;
            }
            this.SendEvent<OnChargeAttackCharging>(e);
        }

        private void ChargeAttackRelease(float totalChargeTime) {
            OnChargeAttackRelease e = new OnChargeAttackRelease() { TotalChargeTime = totalChargeTime};
            if (targetAttackable != null)
            {
                e.AttackableViewController = targetAttackable;
                e.TargetGameObject = targetAttackable.GameObject;
            }
            this.SendEvent<OnChargeAttackRelease>(e);
        }

        /// <summary>
        /// is ult button pressed and ult is ready to cast
        /// </summary>
        /// <returns></returns>
        private bool CheckUlt() {
            if (Input.GetKeyDown(KeyCode.R)) {
                IPlayerModel playerModel = this.GetModel<IPlayerModel>();
                IPlayerConfigurationModel playerConfigurationModel = this.GetModel<IPlayerConfigurationModel>();

                if (playerModel.UltChargeAccumlated.Value >= playerConfigurationModel.MaxUltChargeNeeded) {
                    return true;
                }
            }

            return false;
        }
    }
}
