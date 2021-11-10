using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using UnityEngine;

namespace MikroFramework.SceneEntranceKit
{
    public enum EnvironmentMode {
        Developing,
        Testing,
        Released
    }


    public abstract class EntranceManager : ManagerBehavior {
        [SerializeField]
        protected EnvironmentMode environmentMode;
        public EnvironmentMode EnvironmentMode => environmentMode;

        private static EnvironmentMode sharedMode;
        private static bool modeSet = false;

        void Start() {
            if (!modeSet) {
                sharedMode = environmentMode;
                modeSet = true;
            }
            else {
                environmentMode = sharedMode;
            }

            switch (sharedMode) {
                case EnvironmentMode.Developing:
                    LaunchInDevelopingMode();
                    break;
                case EnvironmentMode.Testing:
                    LaunchInTestingMode();
                    break;
                case EnvironmentMode.Released:
                    LaunchInReleasedMode();
                    break;
            }
        }

        /// <summary>
        /// Code in Developing Mode will only run at the development phrase of the project
        /// </summary>
        protected abstract void LaunchInDevelopingMode();
        /// <summary>
        /// Code in Testing Mode will only run at the testing phrase of the project
        /// </summary>
        protected abstract void LaunchInTestingMode();
        /// <summary>
        /// Code in Released Mode will only run at the released phrase of the project
        /// </summary>
        protected abstract void LaunchInReleasedMode();
    }

}
