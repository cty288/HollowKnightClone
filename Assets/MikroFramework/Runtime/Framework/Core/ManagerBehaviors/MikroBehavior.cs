using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using MikroFramework.Extensions;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using MikroFramework.Utilities;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

namespace MikroFramework {
    public abstract partial class MikroBehavior : MonoBehaviour {
        #region Event System Integrated
        private List<CallbackRecord> registeredEventRecorder = new List<CallbackRecord>();


        

        /// <summary>
        /// Add a listener to the corresponding event type, which must have at least one callback function
        /// The listener is auto removed after the gameobject is destroyed
        /// </summary>
        /// <param name="eventType">The type of the event (create it in the EventType class)</param>
        /// <param name="callBack">A callback function</param>
        public void AddListener(EventType eventType, Action<MikroMessage> callBack)
        {
            EventCenter.AddListener(eventType, callBack).UnRegisterWhenGameObjectDestroyed(this.gameObject);
            CallbackRecord record= CallbackRecord.Allocate();
            record.EventType = eventType;
            record.OnEventReceived = callBack;
           
            registeredEventRecorder.Add(record);
        }


        /// <summary>
        ///  Remove a callback function from the corresponding event type, which must have at least one callback function
        /// </summary>
        /// <param name="eventType">The type of the event (create it in the EventType class)</param>
        /// <param name="callBack">A callback function</param>
        public void RemoveListener(EventType eventType, Action<MikroMessage> callBack)
        {
            var selectedRecords = registeredEventRecorder.FindAll(record =>
               record.EventType == eventType && record.OnEventReceived == callBack);

            selectedRecords.ForEach(record => {
                EventCenter.RemoveListener(record.EventType, record.OnEventReceived);
                registeredEventRecorder.Remove(record);
                record.RecycleToCache();
                
                //record.Recycle();
            });

            selectedRecords.Clear();
        }

        /// <summary>
        ///  Remove all callback function of this object from the corresponding event type, which must have at least one callback function
        /// </summary>
        /// <param name="eventType">The type of the event (create it in the EventType class)</param>
        /// <param name="callBack">A callback function</param>
        public void RemoveListener(EventType eventType)
        {
            var selectedRecords = registeredEventRecorder.FindAll(record =>
                record.EventType == eventType);

            selectedRecords.ForEach(record => {
                EventCenter.RemoveListener(record.EventType, record.OnEventReceived);
                registeredEventRecorder.Remove(record);
                record.RecycleToCache();
                
                //record.Recycle();
            });

            selectedRecords.Clear();
        }

        /// <summary>
        /// Broadcast an event
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="data">data included in the event</param>

        public void Broadcast(EventType eventType, MikroMessage data)
        {
            EventCenter.Broadcast(eventType, data);
            //data.Recycle();
        }


        void OnDestroy()
        {
            foreach (var eventRecord in registeredEventRecorder) {
                eventRecord.RecycleToCache();
            }
            registeredEventRecorder.Clear();
            OnBeforeDestroy();
            
           
        }

        /// <summary>
        /// Use this method instead of Destroy()
        /// </summary>
        protected abstract void OnBeforeDestroy();
        #endregion

        #region Delay 
        /// <summary>
        /// Trigger an event after some seconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="onFinished">triggered event</param>
        public void Delay(float seconds, Action onFinished)
        {
            StartCoroutine(DelayCoroutine(seconds, onFinished));
        }

        public void Delay(float seconds, EventType eventType, MikroMessage data)
        {
            StartCoroutine(DelayCoroutine(seconds, eventType, data));
        }

        private IEnumerator DelayCoroutine(float seconds, Action onFinished)
        {
            yield return new WaitForSeconds(seconds);
            onFinished?.Invoke();
        }

        private IEnumerator DelayCoroutine(float seconds, EventType eventType, MikroMessage data)
        {
            yield return new WaitForSeconds(seconds);
            Broadcast(eventType, data);
        }
        #endregion


    }



}

