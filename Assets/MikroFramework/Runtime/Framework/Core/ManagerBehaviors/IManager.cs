using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using EventType = MikroFramework.Event.EventType;

namespace MikroFramework.Managers
{
    public interface IManager{
        void Broadcast(EventType eventType, MikroMessage data);
        void RemoveListener(EventType eventType);
        void RemoveListener(EventType eventType, Action<MikroMessage> callBack);
        void AddListener(EventType eventType, Action<MikroMessage> callBack);
    }
}
