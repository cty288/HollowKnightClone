using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.IOC;
using MikroFramework.Serializer;
using UnityEngine;

namespace MikroFramework.Examples.CounterApp {
    public class CounterApp : Architecture<CounterApp>
    {
        protected override void Init()
        {
            RegisterSystem<IAchievementSystem>(new AchievementSystem());
            RegisterModel<ICounterModel>(new CounterModel());
            RegisterExtensibleUtility<IStorage>(new PlayerPrefStorage());

        }
    }
}

