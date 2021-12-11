using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.TimeSystem
{
    public interface ITimeSystem : ISystem
    {
        float CurrentSeconds { get; }
        ITimeSystem AddDelayTask(float seconds, Action onFinished);
        void Reset();
        void Pause();
    }

}


