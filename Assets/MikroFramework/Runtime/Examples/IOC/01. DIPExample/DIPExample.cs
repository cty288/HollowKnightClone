using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.IOC;
using UnityEngine;
#if UNITY_EDITOR
namespace MikroFramework.Examples
{
    public class DIPExample : MonoBehaviour {
       public interface IStorage {
           void SaveString(string key, string value = "");
           string LoadString(string key, string defaultValue = "");
       }

       public class PlayPrefStorage : IStorage {
           public void SaveString(string key, string value = "") {
               PlayerPrefs.SetString(key,value);
           }

           public string LoadString(string key, string defaultValue = "") {
               return PlayerPrefs.GetString(key, defaultValue);
           }
       }

       public class EditorPrefStorage : IStorage {

            public void SaveString(string key, string value = "") {
                UnityEditor.EditorPrefs.SetString(key,value);
            }

           public string LoadString(string key, string defaultValue = "") {
               return UnityEditor.EditorPrefs.GetString(key, defaultValue);
           }

        }

       private void Start() {
           IOCContainer container = new IOCContainer();
           container.RegisterInstance<IStorage>(new PlayPrefStorage());

           IStorage storage = container.GetInstance<IStorage>();
           storage.SaveString("name","runtime");

           Debug.Log(storage.LoadString("name"));


           container.RegisterInstance<IStorage>(new EditorPrefStorage());
           storage = container.GetInstance<IStorage>();
           Debug.Log(storage.LoadString("name"));
       }
    }
}
#endif