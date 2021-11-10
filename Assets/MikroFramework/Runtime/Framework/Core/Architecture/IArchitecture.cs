using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Event;

namespace MikroFramework.Architecture
{
    public interface IArchitecture {
        /// <summary>
        /// Get an IUtility object that Inited in the Architecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetUtility<T>() where T : class, IUtility;

        /// <summary>
        /// RegisterInstance a model to the architecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterModel<T>(T instance) where T : IModel;

        /// <summary>
        /// RegisterInstance an IUtility to the architecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterExtensibleUtility<T>(T instance) where T:IUtility;


        /// <summary>
        /// RegisterInstance an ISystem to the architecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="system"></param>
        void RegisterSystem<T>(T system) where T:ISystem;

        /// <summary>
        /// RegisterInstance an IModel to the architecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetModel<T>() where T:class, IModel;

        /// <summary>
        /// Send a ICommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void SendCommand<T>() where T : class, ICommand, new();

        /// <summary>
        /// Send a ICommand object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        void SendCommand<T>(T command) where T : class, ICommand;

        /// <summary>
        /// Get an ISystem that registered to the archiecture
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSystem<T>() where T : class, ISystem;

        /// <summary>
        /// Send a Type event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void SendEvent<T>() where T : new();

        /// <summary>
        /// Send a type event object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        void SendEvent<T>(T e);

        /// <summary>
        /// RegisterInstance a listener to a Type event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onEvent"></param>
        /// <returns></returns>
        IUnRegister RegisterEvent<T>(Action<T> onEvent);

        /// <summary>
        /// Unregister a listener of a Type event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="onEvent"></param>
        void UnRegisterEvent<T>(Action<T> onEvent);


        TResult SendQuery<TResult>(IQuery<TResult> query);

    }
}
