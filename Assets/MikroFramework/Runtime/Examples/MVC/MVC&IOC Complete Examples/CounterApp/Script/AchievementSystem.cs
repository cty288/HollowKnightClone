using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using UniRx;
using UnityEngine;

namespace MikroFramework.Examples.CounterApp{


    public interface IAchievementSystem : ISystem {

    }

    public class AchievementSystem : AbstractSystem, IAchievementSystem {
        private int prevCount;

        protected override void OnInit() {
            var counterModel = this.GetModel<ICounterModel>();

            prevCount = counterModel.Count.Value;


            counterModel.Count.Subscribe(OnCountValueChanged);
        }

        private void OnCountValueChanged(int newCount) {
            if (prevCount < 10 && newCount >= 10)
            {
                Debug.Log("Unlock click 10 times achievement");
            }
            else if (prevCount < 20 && newCount >= 20)
            {
                Debug.Log("Unlock click 20 times achievement");
            }

            prevCount = newCount;
        }
    }
}