using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using UnityEngine;

namespace MikroFramework.Examples
{
    //This is an explanation of Observer Pattern using Type Event system
    public class ObserverPatternWithTypeEventSystemExplanation : MonoBehaviour
    {
        private void Start() {
            Subject subject = new Subject(); //subject is like "broadcaster"

            Observer observerA = new Observer();
            Observer observerB = new Observer();

            subject.DoSomethingThatInterestsObservers();
        }
    }

    public class NotifyEvent { }

    public class Subject {
        //Do things that interests observers (send event)
        public void DoSomethingThatInterestsObservers() {
            Notify();
        }

        void Notify() {
            TypeEventSystem.SendGlobalEvent(new NotifyEvent());
        }
    }

    public class Observer {
        public Observer() {
            Subscribe();
        }

        void Subscribe() {
            TypeEventSystem.RegisterGlobalEvent<NotifyEvent>(Update);
        }

        void Update(NotifyEvent receivedEvent) {
            Debug.Log("Received Notification");
        }

        public void Dispose() {
            TypeEventSystem.UnRegisterGlobalEvent<NotifyEvent>(Update);
        }
    }
}
