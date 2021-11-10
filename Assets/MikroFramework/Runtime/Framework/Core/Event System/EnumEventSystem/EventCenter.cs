using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MikroFramework;
using UnityEngine;

namespace MikroFramework.Event {

    

    public class EventCenter
    {
        private static Dictionary<EventType, Action<MikroMessage>> eventTable = new Dictionary<EventType, Action<MikroMessage>>();
      
        
        /// <summary>
        /// Add a listener to the corresponding event type, which must have at least one callback function
        /// </summary>
        /// <param name="eventType">The type of the event (create it in the EventType class)</param>
        /// <param name="callBack">A callback function, which can have 0-5 parameters</param>
        public static IUnRegister AddListener(EventType eventType, Action<MikroMessage> callBack)
        {
            if (OnListenerAdding(eventType, callBack))
            {
                //多播委托相加
                eventTable[eventType] = eventTable[eventType] + callBack;
            }

            return new EnumEventSystemUnRegister(eventType, callBack);

        }



        /// <summary>
        ///  Remove a callback function from the corresponding event type, which must have at least one callback function
        /// </summary>
        /// <param name="eventType">The type of the event (create it in the EventType class)</param>
        /// <param name="callBack">A callback function with no parameters</param>
        public static void RemoveListener(EventType eventType, Action<MikroMessage> callBack)
        {
            bool success = OnListenerRemoving(eventType, callBack);
            if (success) {
                //移除指定的回调函数
                eventTable[eventType] = eventTable[eventType] - callBack;
                OnListenerRemoved(eventType);
            }
            
        }



        /// <summary>
        /// Remove all callback functions of a specific event
        /// Note: This is irreversible! It will also remove callback functions of this event on other scripts!
        /// </summary>
        /// <param name="eventType"></param>
        public static void RemoveAllListeners(EventType eventType)
        {
            if (eventTable.ContainsKey(eventType))
            {
                eventTable.Remove(eventType);
            }
            else
            {
                throw new Exception(string.Format("Could not find event type {0}", eventType));
            }
        }

        /// <summary>
        /// Broadcast all callback functions with no parameters from an existing eventType
        /// </summary>
        /// <param name="eventType">The type of the event</param>
        public static void Broadcast(EventType eventType,MikroMessage data)
        {
            Action<MikroMessage> d;
            //获取该类型下的委托
            if (eventTable.TryGetValue(eventType, out d))
            {
                Action<MikroMessage> callback = d;
                if (callback != null)
                {
                    //调用委托的所有回调函数
                    callback(data);
                }
                else
                {
                    throw new Exception(string.Format("Failed to broadcast event {0}: The delegate has a different type", eventType));
                }
            }
        }


        private static bool OnListenerAdding(EventType eventType, Action<MikroMessage> callBack)
        {
            //事件库指定事件类型的值为空时, 为该事件类型创建一个新的空间, 并加入委托
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, callBack);
                return false;
            }

            return true;
        }

        private static bool OnListenerRemoving(EventType eventType, Action<MikroMessage> callBack)
        {
            //检查表中是否有该事件类型
            if (eventTable.ContainsKey(eventType))
            {
                Action<MikroMessage> d = eventTable[eventType];
                //判断事件类型是否包含任何回调函数
                if (d == null)
                {
                    //throw new Exception(string.Format("The event {0} does not have corresponding delegate", eventType));
                    //判断移除的事件的类型是否和表中的类型一致
                    return false;
                }

                return true;
            }
            else
            {
                //throw new Exception("Failed to remove the listener: no eventType");
                return false;
            }
        }

        private static void OnListenerRemoved(EventType eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

}







