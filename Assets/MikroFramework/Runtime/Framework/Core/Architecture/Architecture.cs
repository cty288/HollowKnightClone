using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using MikroFramework.IOC;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public abstract class Architecture<T> : IArchitecture where T:Architecture<T>,new() {

        private static T architecture;

        public static IArchitecture Interface {
            get {
                if (architecture == null) {
                    ValidateArchitecture();
                }

                return architecture;
            }
        }
        /// <summary>
        /// Has inited?
        /// </summary>
        private bool inited=false;

        private List<IModel> models = new List<IModel>();

        private List<ISystem> systems = new List<ISystem>();

        public static Action<T> OnRegisterPatch = architecture => { };
        private static void ValidateArchitecture()
        {
            if (architecture == null)
            {
                architecture = new T();
                architecture.Init();

                OnRegisterPatch?.Invoke(architecture);

                foreach (var architectureModel in architecture.models) {
                    architectureModel.Init();
                }

                foreach (var architectureSystem in architecture.systems)
                {
                    architectureSystem.Init();
                }

                architecture.models.Clear();
                architecture.inited = true;
            }
        }

        protected abstract void Init();


        private IOCContainer container = new IOCContainer();

       

        /// <summary>
        /// RegisterInstance a Model to the current architecture, should be called in the Init function of the current architecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public void RegisterModel<T>(T model) where T : IModel {
            model.SetArchitecture(this);
            container.RegisterInstance<T>(model);

            if (!inited) {
                models.Add(model);
            }
            else {
                model.Init();
            }

        }
        /// <summary>
        /// RegisterInstance an Utility to the current architecture, should be called in the Init function of the current architecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void RegisterExtensibleUtility<T>(T instance) where T:IUtility {
            container.RegisterInstance<T>(instance);
        }

        public void RegisterSystem<T>(T system) where T:ISystem {
            system.SetArchitecture(this);
            container.RegisterInstance<T>(system);

            if (!inited)
            {
                systems.Add(system);
            }
            else
            {
                system.Init();
            }
        }

        /// <summary>
        /// Get a model from a system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModel<T>() where T : class, IModel {
            return container.GetInstance<T>();
        }

        public void SendCommand<T>() where T : class, ICommand, new() {
            T command = SafeObjectPool<T>.Singleton.Allocate();

            command.SetArchitecture(this);
            command.Execute();

        }

        public void SendCommand<T>(T command) where T : class, ICommand {
            command.SetArchitecture(this);
            command.Execute();

        }

        public T GetSystem<T>() where T : class, ISystem {
            return container.GetInstance<T>();
        }

        private ITypeEventSystem typeEventSystem = new TypeEventSystem();

        public void SendEvent<T>() where T : new() {
            typeEventSystem.Send<T>();
        }

        public void SendEvent<T>(T e) {
            typeEventSystem.Send<T>(e);
        }

        public IUnRegister RegisterEvent<T>(Action<T> onEvent) {
            return typeEventSystem.Register<T>(onEvent);
        }

        public void UnRegisterEvent<T>(Action<T> onEvent) {
            typeEventSystem.UnRegister<T>(onEvent);
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query) {
            query.SetArchitecture(this);
            return query.Do();
        }

        /// <summary>
        /// Get a specific utility of the current architecture, should be called in the Init() function of Model Objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUtility<T>() where T : class, IUtility {
            return container.GetInstance<T>();
        }
    }
}
