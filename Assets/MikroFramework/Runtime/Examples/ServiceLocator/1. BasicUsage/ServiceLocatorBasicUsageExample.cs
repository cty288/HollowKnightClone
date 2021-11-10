using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MikroFramework.ServiceLocator;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public class InitialContext : AbstractInitialContext
    {
        public override IService LookUp(string name)
        {
            /*
            Assembly assembly = typeof(ServiceLocatorBasicUsageExample).Assembly;
            Type serviceType = typeof(IService);

            IService service = assembly.GetTypes().
                //get all types inherit IService
                Where(t => serviceType.IsAssignableFrom(t) && !t.IsAbstract)
                //create instances
                .Select(t => t.GetConstructors().First<ConstructorInfo>().Invoke(null))
                //cast to IService
                .Cast<IService>().
                //find the service with the target name
                SingleOrDefault(s => s.Name == name);*/

            IService service = null;
            if (name == "bluetooth") {
                service = new BluetoothService();
            }

            return service;
        }
    }

    public class BluetoothService : IService
    {
        public string Name
        {
            get { return "bluetooth"; }
        }
        public void Execute()
        {
            Debug.Log("Bluetooth service start!");
        }
    }


    public class ServiceLocatorBasicUsageExample : MonoBehaviour {
        
        private void Start() {
            InitialContext context = new InitialContext();

            MikroFramework.ServiceLocator.ServiceLocator serviceLocator = new MikroFramework.ServiceLocator.ServiceLocator(context);

            IService bluetoothService = serviceLocator.GetService("bluetooth");

            bluetoothService.Execute();
        }
    }
}
