using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class TypeEventExample : MonoBehaviour
    {
        public struct EventA {

        }

        public struct EventB {
            public int ParamB;
        }

        public interface IEventGroup {
            
        }

        public struct EventC:IEventGroup {
            public int ParamC;
        }
        public struct EventD : IEventGroup
        {

        }

        //for local event. 
        //private TypeEventSystem typeEventSystem = new TypeEventSystem();

        private void Start() {
            //Manually unregister A; not using lambda expression
            TypeEventSystem.RegisterGlobalEvent<EventA>(OnEventA);


            //auto unregister B; use Lambda expression; has parameters
            TypeEventSystem.RegisterGlobalEvent<EventB>(b => {
                Debug.Log("OnEventB: "+b.ParamB);

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            //auto unregister EventGroup; use lambda expression, has parameters
            TypeEventSystem.RegisterGlobalEvent<IEventGroup>(e => {
                Debug.Log(e.GetType());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update() {
            if (Input.GetMouseButton(0)) { //left
                TypeEventSystem.SendGlobalEvent<EventA>();
            }

            if (Input.GetMouseButton(1)) { //right
                TypeEventSystem.SendGlobalEvent(new EventB {
                    ParamB = 123
                });
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                TypeEventSystem.SendGlobalEvent<IEventGroup>(new EventC() {
                    ParamC = 456
                });

                TypeEventSystem.SendGlobalEvent<IEventGroup>(new EventD());
            }
        }

        private void OnEventA(EventA obj) {
            Debug.Log("OnEventA");
        }


        //Manually unregister A
        private void OnDestroy() {
            TypeEventSystem.UnRegisterGlobalEvent<EventA>(OnEventA);
        }
    }
}
