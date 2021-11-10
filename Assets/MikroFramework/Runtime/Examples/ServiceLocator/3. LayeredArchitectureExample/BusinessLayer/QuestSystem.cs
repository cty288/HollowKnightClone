using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IQuestSystem : ISystem {
        void OnEvent(string eventName);
    }
    public class QuestSystem : IQuestSystem
    {
        private int jumpCount {
            get { return PlayerPrefs.GetInt("JUMP_COUNT"); }
            set { PlayerPrefs.SetInt("JUMP_COUNT",value);}
        }


        public void OnEvent(string eventName) {
            if (eventName == "JUMP") {
                jumpCount++;

                if (jumpCount == 1) {
                    Debug.Log("Jump 1 completed");
                }

                if (jumpCount == 5) {
                    Debug.Log("Jump 5 completed");
                }

                if (jumpCount == 10) {
                    Debug.Log("Jump 10 completed");
                }
            }
        }
    }
}
