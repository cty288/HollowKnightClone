using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class MonoSingletonPropertyExample : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/Singletons/04. MonoSingletonPropertyExample", false, 4)]
        private static void MenuClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject("MonoSingletonPropertyExample").AddComponent<MonoSingletonPropertyExample>();
        }
#endif

        private void Start() {
            //new GameObject("MonoObject").AddComponent<MonoObject>();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                MonoObject.Singleton.Num++;
                Debug.Log(MonoObject.Singleton.Num);
            }
        }
    }

    public class MonoObject : MonoBehaviour, ISingleton {
        public int Num = 0;

        public static MonoObject Singleton {
            get {
                return SingletonProperty<MonoObject>.Singleton;
            }
        }

        private void Start() {
            Debug.Log("MonoObject Singleton Start Function");
        }

        void ISingleton.OnSingletonInit() {
            
        }
    }
}
