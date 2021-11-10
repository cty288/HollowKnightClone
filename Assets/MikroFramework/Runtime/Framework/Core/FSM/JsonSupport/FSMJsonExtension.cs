using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.FSM;
using MikroFramework.Serializer;
using UnityEngine;

namespace MikroFramework.FSM
{
    public static class FSMJsonExtension {
        public static void ToJson(this IFSM fsm, string path) {
            Dictionary<string, FSMState> stateDictionary = fsm.StateDict;

            FSMJsonInfo jsonInfo = new FSMJsonInfo();

            foreach (KeyValuePair<string, FSMState> kvp in stateDictionary) {
                
                FSMStateJsonInfo stateJsonInfo = new FSMStateJsonInfo();
                stateJsonInfo.Name = kvp.Key;

                FSMState fsmState = kvp.Value;

                foreach (KeyValuePair<string, FSMTranslation> fsmTranslationKvp in fsmState.TranslationDictionary) {
                    stateJsonInfo.Translations[fsmTranslationKvp.Key] = fsmTranslationKvp.Value.toState.name;
                }

                jsonInfo.FsmJsonInfos.Add(stateJsonInfo);

            }

            string json = AdvancedJsonSerializer.Singleton.Serialize(jsonInfo);
            File.WriteAllText(path,json);

            Debug.Log($"Successfully generate FSM Json file {path}!");
        }

        public static FSM ReadJson(string jsonPath) {
            string json = File.ReadAllText(jsonPath);
            List<FSMStateJsonInfo> FsmJsonInfos = 
                AdvancedJsonSerializer.Singleton.Deserialize<FSMJsonInfo>(json).FsmJsonInfos;
            
            FSM fsm = new FSM();

            foreach (FSMStateJsonInfo fsmStateJsonInfo in FsmJsonInfos) {
                Dictionary<string, string> translations = fsmStateJsonInfo.Translations;

                foreach (KeyValuePair<string, string> translationKvp in translations) {
                    fsm.AddTranslation(fsmStateJsonInfo.Name, translationKvp.Key, translationKvp.Value,null);
                }
            }

            Debug.Log("");

            return fsm;

        }
    }
}
