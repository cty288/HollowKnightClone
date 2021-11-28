using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace HollowKnight
{
    public struct OnWeaponShifted {
        public bool Up;
    }

    public struct OnWeaponDropped {
        public WeaponInfo DroppedWeapon;
    }

    public enum WeaponBuff {

    }

    public interface IWeaponSystem : ISystem {
        WeaponInfo SelectedWeapon { get; }
        List<WeaponInfo> EquippedWeapons { get; }
        WeaponInfo AddWeaponToBackpack(WeaponInfo weaponInfo);
        WeaponInfo DropSelectedWeapon();

        WeaponInfo GetWeaponFromConfig(WeaponName weaponName);

        void NormalAttackWithCurrentWeapon(float timeSinceLastNormalAttack,
            IEnemyViewControllerAttackable AttackableViewController, GameObject targetGameObject);

        void CurrentWeaponCharging(float chargingTime, IEnemyViewControllerAttackable AttackableViewController, GameObject targetGameObject);

        void CurrentWeaponChargeRelease(float totalChargeTime, IEnemyViewControllerAttackable AttackableViewController, GameObject targetGameObject);
        int WeaponCount { get; }

        int BackpackCapacity { get; }
        void ShiftWeapon(bool up);
    }

    public class WeaponSystem : AbstractSystem, IWeaponSystem, ICanSendCommand {

        public List<WeaponInfo> weaponList;

        private int selectIndex = 0;
        private IWeaponConfigModel configModel;

       

        public WeaponInfo SelectedWeapon {
            get {
                if (weaponList.Count > 0) {
                    return weaponList[0];
                }

                return null;
            }
        }

        public List<WeaponInfo> EquippedWeapons
        {
            get {
                return weaponList;
            }
        }

       
        public int WeaponCount {
            get {
                return weaponList.Count;
            }
        }
        public int BackpackCapacity { get; } = 4;


        protected override void OnInit() {
            configModel = this.GetModel<IWeaponConfigModel>();

            weaponList = new List<WeaponInfo>();

            this.RegisterEvent<OnWeaponAddedToBackpack>(OnWeaponAdded);
            this.RegisterEvent<OnNormalAttack>(OnNormalAttackTriggers);
            this.RegisterEvent<OnChargeAttackCharging>(OnChargeAttackCharging);
            this.RegisterEvent<OnChargeAttackRelease>(OnChargeAttackReleased);
        }

        private void OnChargeAttackReleased(OnChargeAttackRelease e) { 
            CurrentWeaponChargeRelease(e.TotalChargeTime,e.AttackableViewController,
                e.TargetGameObject);
            Debug.Log($"Charge Attack to enemy released: {e.TargetGameObject.name}.");
        }

        private void OnChargeAttackCharging(OnChargeAttackCharging e) {
            CurrentWeaponCharging(e.ChargeTime,e.AttackableViewController,e.TargetGameObject);
            Debug.Log($"Charge Attack to enemy Charging: {e.TargetGameObject.name}.");
        }

        private void OnNormalAttackTriggers(OnNormalAttack e) {
            NormalAttackWithCurrentWeapon(e.TimeSinceLastNormalAttack,e.AttackableViewController,
                e.TargetGameObject);
           
            Debug.Log($"Normal Attack to enemy: {e.TargetGameObject.name}.");
        }


        private void OnWeaponAdded(OnWeaponAddedToBackpack e) {
            AddWeaponToBackpack(e.WeaponInfo);
        }

        /// <summary>
        /// Return the dropped weapon (if the backpack is full)
        /// </summary>
        /// <returns></returns>
        public WeaponInfo AddWeaponToBackpack(WeaponInfo weaponInfo) {
            WeaponInfo droppedWeapon = null;

            if (weaponList.Count >= BackpackCapacity) {
                droppedWeapon = DropSelectedWeapon();
            }

            weaponList.Insert(0, weaponInfo);
            Debug.Log($"Added {weaponInfo.Name.Value}, With Capacity {weaponInfo.BulletCount.Value}");
            weaponInfo.OnBulletCountChange += OnWeaponBulletChange;
            return droppedWeapon;
        }

        private void OnWeaponBulletChange(WeaponInfo weapon, int oldBullet, int newBullet) {
            if (newBullet <= 0) {
                //use up all bullets
                weaponList.Remove(weapon);
                Debug.Log($"Weapon {weapon.Name} used up all bullets");
            }
        }

        public WeaponInfo DropSelectedWeapon() {
            WeaponInfo droppedWeapon = null;

            if (WeaponCount > 0) {
                droppedWeapon = weaponList[0];
                weaponList.RemoveAt(0);
                this.SendEvent<OnWeaponDropped>(new OnWeaponDropped(){DroppedWeapon = droppedWeapon});
                if (SelectedWeapon != null) {
                    Debug.Log($"Weapon dropped. Current Weapon: {SelectedWeapon.Name.Value}; Bullet: {SelectedWeapon.BulletCount.Value}");
                }
                
            }


            return droppedWeapon;
        }

     
        public void ShiftWeapon(bool up) {
            if (WeaponCount > 1) {
                int lastIndex = WeaponCount - 1;
                WeaponInfo first = weaponList[0];
                WeaponInfo last = weaponList[lastIndex];

                if (up) {
                    weaponList.RemoveAt(lastIndex);
                    weaponList.Insert(0, last);
                }
                else {
                    weaponList.RemoveAt(0);
                    weaponList.Add(first);
                }

                this.SendEvent<OnWeaponShifted>(new OnWeaponShifted() {Up = up});
                Debug.Log($"Weapon Shifted. Current Weapon: {SelectedWeapon.Name}");
            }
        }

        public WeaponInfo GetWeaponFromConfig(WeaponName weaponName) {
            WeaponConfigItem configItem = configModel.GetWeaponByName(weaponName);

            WeaponInfo weaponInfo = new WeaponInfo(configItem.WeaponName,
                configItem.TypeConfigItem.Type, configItem.TypeConfigItem.AttackSkill,
                configItem.TypeConfigItem.AttackDamage, configItem.TypeConfigItem.AttackFreq,configItem.TypeConfigItem.ChargeAttackSkill,
                configItem.TypeConfigItem.ChargeAttackTime, configItem.TypeConfigItem.ChargeAttackDamage,
                configItem.TypeConfigItem.Ult, configItem.TypeConfigItem.UltChargeTime,
                configItem.TypeConfigItem.UltDamage, configItem.WeaponCapacity, configItem.WeaponCapacity);

            return weaponInfo;
        }

        
        public void NormalAttackWithCurrentWeapon(float timeSinceLastNormalAttack,
            IEnemyViewControllerAttackable AttackableViewController, GameObject targetGameObject) {
            WeaponInfo weapon = SelectedWeapon;

            if (weapon != null) {
                if (timeSinceLastNormalAttack >= weapon.AttackFreq.Value)
                {
                    IWeaponCommand command = ConfigureAttackCommand(weapon.AttackSkill.Value, weapon,
                        timeSinceLastNormalAttack, AttackableViewController, targetGameObject);
                    this.SendCommand(command);
                }
            }
            
        }

        private IWeaponCommand ongoingChargingCommand = null;

        public void CurrentWeaponCharging(float chargingTime, IEnemyViewControllerAttackable AttackableViewController, GameObject targetGameObject) {
            WeaponInfo weapon = SelectedWeapon;

            IWeaponCommand command = null;

            if (ongoingChargingCommand == null) {
                command = ConfigureAttackCommand(weapon.ChargeAttackSkill.Value, weapon,
                    chargingTime, AttackableViewController, targetGameObject, false);
                ongoingChargingCommand = command;
            }
            else {
                command = ongoingChargingCommand;
                command.Time = chargingTime;
                command.Released = false;
            }
           

            this.SendCommand(command);
        }

        public void CurrentWeaponChargeRelease(float totalChargeTime, IEnemyViewControllerAttackable AttackableViewController, GameObject targetGameObject) {
            WeaponInfo weapon = SelectedWeapon;
            IWeaponCommand command = null;

            
                if (ongoingChargingCommand == null)
                {
                    command = ConfigureAttackCommand(weapon.ChargeAttackSkill.Value, weapon,
                        totalChargeTime, AttackableViewController, targetGameObject, true);
                }
                else
                {
                    command = ongoingChargingCommand;
                    command.Time = totalChargeTime;
                    command.Released = true;
                }

                this.SendCommand(command);
            
           

            ongoingChargingCommand = null;
        }

        private IWeaponCommand ConfigureAttackCommand(IWeaponCommand command, WeaponInfo weapon, float time,
            IEnemyViewControllerAttackable attackable, GameObject targetGameObject, bool released = true) {
            
            IWeaponCommand cmd = command.Clone();
            cmd.WeaponInfo = weapon;
            cmd.Time = time;
            cmd.TargetAttackableViewController = attackable;
            cmd.TargetGameObject = targetGameObject;
            cmd.Released = released;
            return cmd;
        }

        private WeaponInfo GetWeaponFromConfig(WeaponName weaponName, int bulletInGun)
        {
            WeaponConfigItem configItem = configModel.GetWeaponByName(weaponName);
            WeaponInfo weaponInfo = new WeaponInfo(configItem.WeaponName,
                configItem.TypeConfigItem.Type, configItem.TypeConfigItem.AttackSkill,
                configItem.TypeConfigItem.AttackDamage, configItem.TypeConfigItem.AttackFreq,
                configItem.TypeConfigItem.ChargeAttackSkill,
                configItem.TypeConfigItem.ChargeAttackTime, configItem.TypeConfigItem.ChargeAttackDamage,
                configItem.TypeConfigItem.Ult, configItem.TypeConfigItem.UltChargeTime,
                configItem.TypeConfigItem.UltDamage, bulletInGun, configItem.WeaponCapacity);
            return weaponInfo;
        }

        
        
        
    }
}
