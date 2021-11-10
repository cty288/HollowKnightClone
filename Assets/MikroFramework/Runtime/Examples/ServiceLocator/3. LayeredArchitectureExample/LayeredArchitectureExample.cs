using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public class LayeredArchitectureExample : MonoBehaviour {
        private ILoginController loginController;
        private IUserInputManager userInputManager;

        private void Start() {
            loginController = ArchitectureConfig.Architecture.LogicLayer.GetModule<ILoginController>();
            userInputManager = ArchitectureConfig.Architecture.LogicLayer.GetModule<IUserInputManager>();

            loginController.Login();
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                userInputManager.OnInput(KeyCode.Space);
            }
        }
    }
}
