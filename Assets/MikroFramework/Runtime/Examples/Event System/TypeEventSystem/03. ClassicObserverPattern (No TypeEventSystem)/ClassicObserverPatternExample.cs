using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ClassicObserverPattern
{
    public class ClassicObserverPatternExample : MonoBehaviour
    {
        private void Start() {
            ConcreteSubject subject = new ConcreteSubject();
            ConcreteObserver observer = new ConcreteObserver(subject);

            subject.Attach(observer);

            subject.SetState("Test");
        }
    }

    public abstract class Subject {
        private List<Observer> observers = new List<Observer>();

        //Add observers
        public void Attach(Observer observer) {
            observers.Add(observer);
        }

        public void Detach(Observer observer) {
            observers.Remove(observer);
        }

        public void Notify() {
            observers.ForEach(observer=>observer.Update());
        }
    }

    public abstract class Observer {
        public abstract void Update();
    }

    public class ConcreteSubject : Subject {
        //Thing that Observers are interested
        private string subjectState;

        public void SetState(string state) {
            this.subjectState = state;
            Notify();
        }

        public string GetState() {
            return this.subjectState;
        }
    }

    public class ConcreteObserver : Observer {
        private ConcreteSubject subject = null;

        public ConcreteObserver(ConcreteSubject subject) {
            this.subject = subject;
        }

        public override void Update() {
            Debug.Log("ConcreteObserver.Update");
            Debug.Log("Current subject: "+ subject.GetState());
        }
    }
}
