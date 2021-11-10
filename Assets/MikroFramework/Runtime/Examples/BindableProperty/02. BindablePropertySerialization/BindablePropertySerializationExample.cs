using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MikroFramework.BindableProperty;
using MikroFramework.Serializer;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class BindablePropertySerializationExample : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/BindableProperty/2. SerializeBindableProperty", false, 1)]
        private static void OnClicked() {
            
           /* PlayerInfo playerInfo = new PlayerInfo();

            playerInfo.Age.Value = 100;
            playerInfo.Name.Value = "Test";
            playerInfo.State.Value = new PlayerState() {hp = 50, mp = 90};

            string jsonString= AdvancedJsonSerializer.Singleton.Serialize(playerInfo);
            File.WriteAllText(Application.dataPath + "/info.json", jsonString);*/


           string json=  File.ReadAllText(Application.dataPath +  "/info.json");
           PlayerInfo playerInfo = AdvancedJsonSerializer.Singleton.Deserialize<PlayerInfo>(json);

           Debug.Log(playerInfo.State.Value.hp);


           /*Info info = new Info();
           info.Age.Value = 100;
           info.Name.Value = "Info";
           string jsonString = AdvancedJsonSerializer.Singleton.Serialize(info);
           File.WriteAllText(Application.dataPath+"/info.json", jsonString);
           Debug.Log("Write success!");*/
        }
#endif

        [Serializable]
        public class Info {
            public BindableProperty<string> Name = new BindableProperty<string>() { Value = "" };
            public BindableProperty<int> Age  = new BindableProperty<int>() { Value = 0 };
        }

      

    }
}
