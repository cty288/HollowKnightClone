#if UNITY_EDITOR


using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.ResKit
{
    public class SimulationModeMenu {

        private const string simulationModePath = "MikroFramework/Framework/ResKit/Simulation Mode";

        private static bool SimulationMode {
            get {
                return ResManager.SimulationMode;
            }
            set {
                ResManager.SimulationMode = value;
            }
        }

        [MenuItem(simulationModePath)]
        private static void ToggleSimulationMode() {
            SimulationMode = !SimulationMode;
        }

        [MenuItem(simulationModePath, true)]
        private static bool ToggleSimulationModeValidate() {
            Menu.SetChecked(simulationModePath,SimulationMode);
            return true;
        }

    }
}

#endif