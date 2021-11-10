using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.Serializer
{

    /// <summary>
    /// Default Json Serializer uses JsonUtility to serialize objects. This can accomplish most Serialization task,
    /// but it does not work well with classes that contains List, Dictionary, or Generic types. Use AdvanceJsonSerializer
    /// if you want to serialize those types.
    /// </summary>
    public class DefaultJsonSerializer : MikroSingleton<DefaultJsonSerializer>, IJsonSerializer
    {
        private DefaultJsonSerializer() { }

        public string Serialize<T>(T obj) where T : class {
            return JsonUtility.ToJson(obj,true);
        }

        public T Deserialize<T>(string json) where T : class {
            return JsonUtility.FromJson<T>(json);
        }
    }
}
