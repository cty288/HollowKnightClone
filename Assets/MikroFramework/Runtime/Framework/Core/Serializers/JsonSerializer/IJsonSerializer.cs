using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework.Serializer
{
    public interface IJsonSerializer:IUtility {
        /// <summary>
        /// Serialize an object to Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize<T>(T obj) where T : class;

        /// <summary>
        /// Deserialize an object from Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json) where T : class;
    }
}
