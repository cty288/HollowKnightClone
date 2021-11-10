using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.IOC;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class IOCExample : MonoBehaviour {
        private void Start() {
            IOCContainer container = new IOCContainer();
            container.RegisterInstance<IBluetoothManager>(new BlueToothManager());

            IBluetoothManager blueToothManager = container.GetInstance<IBluetoothManager>();

            blueToothManager.Connect();

        }

    }

    public interface IBluetoothManager {
        void Connect();
    }
    public class BlueToothManager:IBluetoothManager {
        public void Connect() {
            Debug.Log("Bluetooth connect success");
        }
    }
}
