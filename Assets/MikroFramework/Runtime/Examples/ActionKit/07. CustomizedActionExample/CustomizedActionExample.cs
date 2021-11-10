using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Pool;
using UnityEngine;
using UnityEngine.Assertions;

namespace MikroFramework.Examples
{
    public class CustomizedActionExample : MonoBehaviour
    {
        public class DEFAction : MikroAction {
            private float timer = 0;
            private string msg = "";

            protected override void OnBegin() {
                Debug.Log("Begin");
            }

            protected override void OnExecuting() {
                timer += Time.deltaTime;
                if (timer > 5) {
                    Debug.Log(msg);
                    Finish();
                }
            }

            protected override void OnEnd() {
                Debug.Log("End");
            }

            protected override void OnDispose() {
                timer = 0;
                msg = "";
            }

            protected override void RecycleBackToPool() {
                SafeObjectPool<DEFAction>.Singleton.Recycle(this);
            }

            public static DEFAction Allocate(string msg) {
                DEFAction action = SafeObjectPool<DEFAction>.Singleton.Allocate();
                action.msg = msg;
                return action;
            }
        }



        class ABCAction : MikroAction
        {


            private float startSeconds = 0;

            public Action finishCallback = () => { };


            protected override void OnBegin()
            {
                startSeconds = Time.time;
            }

            protected override void OnExecuting()
            {
                if (Time.time - startSeconds > 2.0f)
                {
                    Finished.Value = true;
                    finishCallback();

                }

            }

            protected override void RecycleBackToPool()
            {
                SafeObjectPool<ABCAction>.Singleton.Recycle(this);
            }
        }



        // Start is called before the first frame update
        void Start()
        {
            /*
            ABCAction action = new ABCAction()
            {
                finishCallback = () => { Debug.Log("Finished");}

            };

            this.Execute(action);*/



            DEFAction action=DEFAction.Allocate("MSG");
            action.SetAutoRecycle(false);
          
            action.Execute();

            action.RecycleToCache();
        }

      
    }
}
