using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples
{
    public interface ICanSayHello {
        void SayHello();
        void SayOther();
    }
    public class InterfaceDesignExample :MonoBehaviour, ICanSayHello {
        public void SayHello() {
            Debug.Log("Hello");
        }
        
        void ICanSayHello.SayOther() {
            Debug.Log("Other");
        }

        void Start() {
            this.SayHello();
            (this as ICanSayHello).SayOther();
        }
    }
}
