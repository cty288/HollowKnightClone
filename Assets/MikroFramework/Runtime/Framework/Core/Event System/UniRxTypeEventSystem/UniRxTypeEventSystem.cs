using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MikroFramework.Event
{

    public class UniRxTypeEventSystem
    {
        public interface IRegisterations
        {
        }

        public class Registerations<T> : IRegisterations {
            public Subject<T> Subject = new Subject<T>();
        }



        private static Dictionary<Type, IRegisterations> eventRegisteration = new Dictionary<Type, IRegisterations>();




        /// <summary>
        /// Send a local event. Use static functions (global events) if you are not using an architecture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Send<T>() where T : new()
        {
            T e = new T();
            Send<T>(e);
        }

        /// <summary>
        /// Send a local event. Use static functions (global events) if you are not using an architecture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        public static void Send<T>(T e)
        {
            Type type = typeof(T);
            IRegisterations registerations;

            if (eventRegisteration.TryGetValue(type, out registerations))
            {
                (registerations as Registerations<T>).Subject.OnNext(e);
            }
        }

        /// <summary>
        /// RegisterInstance a local event. Use static functions (global events) if you are not using an architecture.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onEvent"></param>
        /// <returns></returns>

        public static Subject<T> GetEvent<T>()
        {
            Type type = typeof(T);
            IRegisterations registerations;

            if (eventRegisteration.TryGetValue(type, out registerations))
            {

            }
            else
            {
                registerations = new Registerations<T>();
                eventRegisteration.Add(type, registerations);
            }

            return (registerations as Registerations<T>).Subject;

        }

    }
}
