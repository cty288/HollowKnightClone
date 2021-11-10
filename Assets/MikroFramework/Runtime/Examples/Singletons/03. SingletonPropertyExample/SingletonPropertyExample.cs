using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class SingletonPropertyExample : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/Singletons/03. SingletonPropertyExample", false, 3)]
        private static void MenuClicked() {
            Debug.Log(User.Singleton.Name+"   "+User.Singleton.Age);
            User.Singleton.Name = "test2";
            User.Singleton.Age++;
            Debug.Log(User.Singleton.Name + "   " + User.Singleton.Age);
        }
#endif

    }

    public class User:ISingleton {
        public string Name = "";
        public int Age = 20;

        private User() {

        }
        public static User Singleton {
            get {
                return SingletonProperty<User>.Singleton;
            }
        }

        void ISingleton.OnSingletonInit() {
            
        }
    }
}
