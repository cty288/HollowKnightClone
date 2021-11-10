using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using MikroFramework.Serializer;
using UnityEngine;

namespace MikroFramework.Utilities
{
    // List<T>
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> target;
        public List<T> ToList() { return target; }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }

    // Dictionary<TKey, TValue>
    [Serializable]
    public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys;
        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> ToDictionary() { return target; }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }
    public class SerializationUtility : MonoBehaviour
    {
        /// <summary>
        /// Serialize a list to Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string ListToJson<T>(List<T> l)
        {
            return DefaultJsonSerializer.Singleton.Serialize(new Serialization<T>(l));
        }

        /// <summary>
        /// Deserialize a list from Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<T> ListFromJson<T>(string str)
        {
            return DefaultJsonSerializer.Singleton.Deserialize<Serialization<T>>(str).ToList();
        }

        /// <summary>
        /// Serialize a Dictionary to Json
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string DicToJson<TKey, TValue>(Dictionary<TKey, TValue> dic)
        {
            return DefaultJsonSerializer.Singleton.Serialize(new Serialization<TKey, TValue>(dic));
        }

        /// <summary>
        /// Deserialize a dictionary from Json
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> DicFromJson<TKey, TValue>(string str)
        {
            return DefaultJsonSerializer.Singleton.Deserialize<Serialization<TKey, TValue>>(str).ToDictionary();
        }


        public static string StringToBinary(string str)
        {
            byte[] data = Encoding.Unicode.GetBytes(str);
            StringBuilder sb = new StringBuilder(data.Length * 8);
            foreach (byte item in data)
            {
                sb.Append(Convert.ToString(item, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        public static string BinaryToString(string str)
        {
            System.Text.RegularExpressions.CaptureCollection cs = System.Text.RegularExpressions.Regex.Match(str, @"([01]{8})+").Groups[1].Captures;
            byte[] data = new byte[cs.Count];
            for (int i = 0; i < cs.Count; i++)
            {
                data[i] = Convert.ToByte(cs[i].Value, 2);
            }
            return Encoding.Unicode.GetString(data, 0, data.Length);
        }

        /// <summary>
        /// Serialize a serializable object to binary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serialize"></param>
        /// <param name="path">Path to save</param>
        public static void BinarySerialize<T>(T serialize, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, serialize);
            }
        }

        /// <summary>
        /// Deserialize a binary file to a serializable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T BinaryDeserialize<T>(string path)
        {
            using (FileStream fs = new FileStream(Application.dataPath + "/test.bytes", FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                T testSerialize = (T)bf.Deserialize(fs);
                return testSerialize;
            }
        }

        /// <summary>
        /// Serialize a XML serializable object to XML file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedObject"></param>
        /// <param name="path"></param>
        public static void XmlSerialize<T>(T serializedObject,string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create,
                FileAccess.ReadWrite, FileShare.ReadWrite);

            StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            XmlSerializer xml = new XmlSerializer(serializedObject.GetType());

            xml.Serialize(sw, serializedObject);
            sw.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Deserialize a XML file to a XML serializable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                T serializable = (T)xs.Deserialize(fs);
                return serializable;
            }
        }
    }
}
