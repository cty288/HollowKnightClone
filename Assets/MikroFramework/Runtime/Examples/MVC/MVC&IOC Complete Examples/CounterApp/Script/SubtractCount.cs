using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Examples.CounterApp {
    public class SubtractCount : AbstractCommand<SubtractCount>
    {

        protected override void OnExecute()
        {
            this.GetModel<ICounterModel>().Count.Value--;
        }
    }

}
