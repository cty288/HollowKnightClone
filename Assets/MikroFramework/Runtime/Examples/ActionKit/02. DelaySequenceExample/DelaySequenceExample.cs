using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class DelaySequenceExample : MonoBehaviour
    {
		void Start()
        {

            var sequenecNode = new Sequence();

            sequenecNode.AddAction(DelayAction.Allocate(1.0f, () => Debug.Log("delay 1 sec" + DateTime.Now)));
            sequenecNode.AddAction(DelayAction.Allocate(1.0f, () => Debug.Log("delay 1 sec" + DateTime.Now)));
            sequenecNode.AddAction(DelayAction.Allocate(1.0f, () => Debug.Log("delay 1 sec" + DateTime.Now)));
            sequenecNode.AddAction(DelayAction.Allocate(1.0f, () => Debug.Log("delay 1 sec" + DateTime.Now)));
            sequenecNode.AddAction(DelayAction.Allocate(1.0f, () => Debug.Log("delay 1 sec" + DateTime.Now)));
            sequenecNode.AddAction(DelayAction.Allocate(1.0f, () => Debug.Log("delay 1 sec" + DateTime.Now)));

            //sequenecNode.Execute();
            //or
            this.Execute(sequenecNode);
        }
	}
}
