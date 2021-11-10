using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface ILoginController : ILogicController {
        void Login();
    }
    public class LoginController : ILoginController {
        public void Login() {
            IAccountSystem accountSystem =
                ArchitectureConfig.Architecture.BusinessModuleLayer.GetModule<IAccountSystem>();

            accountSystem.Login("hello","23333", (success) => {
                if (success) {
                    Debug.Log("Login Success");
                }
            });
        }
    }
}
