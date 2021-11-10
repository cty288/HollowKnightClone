using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Event;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.Utilities
{
    //debugger for Unity Editor. 
    public partial class Debugger:MonoBehaviour {
        
        protected struct PoolInfo {
            public int Value;
            public int MaxValue;
        }

        protected Dictionary<string, PoolInfo> safeObjectPoolDict = new Dictionary<string, PoolInfo>();

        private void Start() {
            DontDestroyOnLoad(this.gameObject);
            AddEventListeners();
        }

        public virtual void AddEventListeners() {
            TypeEventSystem.RegisterGlobalEvent<OnPoolValueChanged>(PoolValueChangeEvent)
                .UnRegisterWhenGameObjectDestroyed(this.gameObject);
        }

        private void PoolValueChangeEvent(OnPoolValueChanged ev) {
            safeObjectPoolDict[ev.PoolName] = new PoolInfo(){Value = ev.Value, MaxValue = ev.MaxValue};
        }

        public virtual void OnGUI() {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.F2)) {
                GUILayout.BeginVertical("box");
                
                //--------------------------Pool-------------------------------
                AddNewDebugCatalogue("ObjectPool",20,Color.white);
                foreach (KeyValuePair<string,PoolInfo> kvp in safeObjectPoolDict) {
                    GUILayout.Label($"[SafeObjectPool] - {kvp.Key} | Active object count: {kvp.Value.Value} / " +
                                    $"{kvp.Value.MaxValue}");
                }
            }
#endif
        }

        /// <summary>
        /// Add a new catalogue to the debugger. Call this function in OnGUI()
        /// </summary>
        protected void AddNewDebugCatalogue(string name, int fontSize, Color catalogueColor) {
            GUILayout.Label($"---------------------------{name}---------------------------",
                new GUIStyle()
                {
                    fontSize = fontSize,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState()
                    {
                        textColor = catalogueColor
                    }
                });
        }
    }
}
