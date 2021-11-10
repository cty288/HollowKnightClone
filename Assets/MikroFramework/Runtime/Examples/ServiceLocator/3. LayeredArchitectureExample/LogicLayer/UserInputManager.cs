using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IUserInputManager : ILogicController {
        void OnInput(KeyCode keyCode);
    }
    public class UserInputManager : IUserInputManager {
        public void OnInput(KeyCode keyCode) {
            IQuestSystem questSystem = ArchitectureConfig.Architecture.BusinessModuleLayer.GetModule<IQuestSystem>();
            questSystem.OnEvent("JUMP");
            Debug.Log("Input: "+keyCode);
        }
    }
}
