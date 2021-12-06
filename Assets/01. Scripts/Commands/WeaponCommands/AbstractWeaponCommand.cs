using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight {
    public abstract class AbstractWeaponCommand<T> : IWeaponCommand where T: AbstractWeaponCommand<T>,new() {
        private IArchitecture architectureModel;


        public AbstractWeaponCommand(WeaponInfo info) {
            WeaponInfo = info;
        }
        public AbstractWeaponCommand() { }


        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return architectureModel;
        }



        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architectureModel = architecture;
        }


        void ICommand.Execute()
        {
            OnExecute();
        }

        /// <summary>
        /// Execute this command
        /// </summary>
        /// <param name="parameters"></param>
        protected abstract void OnExecute();
        void IPoolable.OnRecycled() {
            
        }

        public bool IsRecycled { get; set; }
        public void RecycleToCache() {
            
        }

        public IEnemyViewControllerAttackable TargetAttackableViewController { get; set; }
        public GameObject TargetGameObject { get; set; }
        public float Time { get; set; }
        public bool Released { get; set; }

        public Vector2 TargetPosition { get; set; }

        public WeaponInfo WeaponInfo { get; set; }
        
        public IWeaponCommand Clone() {
            IWeaponCommand command = new T();
            return command;
        }
    }

   
}