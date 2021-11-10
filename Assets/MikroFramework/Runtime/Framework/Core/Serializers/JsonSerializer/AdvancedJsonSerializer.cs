using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Serializer;
using MikroFramework.Singletons;
using Newtonsoft.Json;
using UnityEngine;

namespace MikroFramework.Serializer
{
    /// <summary>
    /// AdvancedJsonSerializer is based on Newtonsoft.Json. It works well with List, Dictionary, and Generic types
    /// </summary>
    public class AdvancedJsonSerializer : MikroSingleton<AdvancedJsonSerializer>, IJsonSerializer
    {
        private AdvancedJsonSerializer() { }

        public string Serialize<T>(T obj) where T : class {
            return JsonConvert.SerializeObject(obj,Formatting.Indented);
        }

        public T Deserialize<T>(string json) where T : class {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
