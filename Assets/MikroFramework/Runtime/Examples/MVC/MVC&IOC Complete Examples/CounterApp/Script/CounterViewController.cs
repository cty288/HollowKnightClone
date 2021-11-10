using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MikroFramework.Examples.CounterApp {

    public class CounterViewController : MonoBehaviour, IController {
        private ICounterModel counterModel;


        private void Start() {
            counterModel = this.GetModel<ICounterModel>();

            counterModel.Count.Select(count=>count.ToString()).
                SubscribeToText(transform.Find("Text").GetComponent<Text>()).AddTo(this.gameObject);


            transform.Find("AddButton").GetComponent<Button>().OnClickAsObservable().Subscribe((_) => {
                this.SendCommand<AddCountCommand>();
            });

            transform.Find("SubtractButton").GetComponent<Button>().OnClickAsObservable().Subscribe((_) => {
                this.SendCommand<SubtractCount>();
            });

        }

        
        IArchitecture IBelongToArchitecture.GetArchitecture() {
            return CounterApp.Interface;
        }
    }

    


}