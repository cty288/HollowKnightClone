using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Event
{
    
    public class TypeEventSystem : ITypeEventSystem
    {
        public interface IRegisterations {
        }

        public class Registerations<T> : IRegisterations {
            public Action<T> OnEvent = e => {

            };
        }


        private static readonly ITypeEventSystem globalEventSystem = new TypeEventSystem();

        private Dictionary<Type, IRegisterations> eventRegisteration = new Dictionary<Type, IRegisterations>();

        
        /// <summary>
        /// RegisterInstance a global event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onEvent">callback function</param>
        /// <returns></returns>
        public static IUnRegister RegisterGlobalEvent<T>(Action<T> onEvent) {
            return globalEventSystem.Register<T>(onEvent);
        }

        /// <summary>
        /// Send a global event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void SendGlobalEvent<T>() where T : new() {
            globalEventSystem.Send<T>();
        }

        /// <summary>
        /// Send a global event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        public static void SendGlobalEvent<T>(T e) {
            globalEventSystem.Send<T>(e);
        }

        /// <summary>
        /// Unregister a global event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onEvent">callback function</param>
        public static void UnRegisterGlobalEvent<T>(Action<T> onEvent) {
            globalEventSystem.UnRegister<T>(onEvent);
        }


        /// <summary>
        /// Send a local event. Use static functions (global events) if you are not using an architecture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Send<T>() where T : new() {
            T e = new T();
            Send<T>(e);
        }

        /// <summary>
        /// Send a local event. Use static functions (global events) if you are not using an architecture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        public void Send<T>(T e) {
            Type type = typeof(T);
            IRegisterations registerations;

            if (eventRegisteration.TryGetValue(type, out registerations)) {
                (registerations as Registerations<T>).OnEvent.Invoke(e);
            }
        }

        /// <summary>
        /// RegisterInstance a local event. Use static functions (global events) if you are not using an architecture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onEvent"></param>
        /// <returns></returns>

        public IUnRegister Register<T>(Action<T> onEvent) {
            Type type = typeof(T);
            IRegisterations registerations;

            if (eventRegisteration.TryGetValue(type, out registerations)) {

            }
            else {
                registerations = new Registerations<T>();
                eventRegisteration.Add(type,registerations);
            }

            (registerations as Registerations<T>).OnEvent += onEvent;


            return new TypeEventSystemUnRegister<T>(this, onEvent);
              
        }

        /// <summary>
        /// UnRegister a local event. Use static functions (global events) if you are not using an architecture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onEvent"></param>
        public void UnRegister<T>(Action<T> onEvent) {
            Type type = typeof(T);
            IRegisterations registerations;

            if (eventRegisteration.TryGetValue(type, out registerations)) {
                (registerations as Registerations<T>).OnEvent -= onEvent;
            }
        }
    }
}
