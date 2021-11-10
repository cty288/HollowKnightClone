using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Examples.CounterApp {

    public interface IStorage : IUtility {
        void SaveInt(string key, int value = 0);
        int LoadInt(string key, int defaultValue = 0);
    }

    public class PlayerPrefStorage : IStorage {
        public void SaveInt(string key, int value = 0) {
            PlayerPrefs.SetInt(key, value);
        }

        public int LoadInt(string key, int defaultValue = 0) {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }

#if UNITY_EDITOR
    public class EditorStorage : IStorage {

        public void SaveInt(string key, int value = 0) {
            UnityEditor.EditorPrefs.SetInt(key, value);
        }

        public int LoadInt(string key, int defaultValue = 0) {
            return UnityEditor.EditorPrefs.GetInt(key, defaultValue);
        }

    }
#endif
}