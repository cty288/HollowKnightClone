using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Event
{
    public class UnRegisterDestroyTrigger : MonoBehaviour
    {
        private HashSet<IUnRegister> unRegisters = new HashSet<IUnRegister>();

        public void AddUnRegister(IUnRegister unRegister)
        {
            unRegisters.Add(unRegister);
        }

        private void OnDestroy()
        {
            foreach (IUnRegister unRegister in unRegisters)
            {
                unRegister.UnRegister();
            }
            unRegisters.Clear();
        }
    }


    public static class UnRegisterExtension
    {
        /// <summary>
        /// Unregister this listener when a specific gameObject is destroyed
        /// </summary>
        /// <param name="unRegister"></param>
        /// <param name="gameObject"></param>
        public static void UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister, GameObject gameObject)
        {
            UnRegisterDestroyTrigger trigger = gameObject.GetComponent<UnRegisterDestroyTrigger>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<UnRegisterDestroyTrigger>();
            }

            trigger.AddUnRegister(unRegister);
        }
    }

}
