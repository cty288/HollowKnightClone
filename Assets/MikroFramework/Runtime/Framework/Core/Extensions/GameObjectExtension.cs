using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MikroFramework.Extensions
{
    public static partial class GameObjectExtension
    {
        /// <summary>
        /// Set the target gameobject to active
        /// </summary>
        /// <param name="obj">target gameobject</param>
        public static void Show(this GameObject obj)
        {
            obj.SetActive(true);
        }

        /// <summary>
        /// Set the target GameObject to inactive
        /// </summary>
        /// <param name="obj">target gameobject</param>
        public static void Hide(this GameObject obj)
        {
            obj.SetActive(false);
        }

        /// <summary>
        /// Set the GameObject of the target Transform component to active
        /// </summary>
        /// <param name="transform"></param>
        public static void Show(this Transform transform)
        {
            transform.gameObject.SetActive(true);
        }

        /// <summary>
        /// Set the GameObject of the target Transform component to inactive
        /// </summary>
        /// <param name="transform"></param>
        public static void Hide(this Transform transform)
        {
            transform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Set the GameObject of the target Transform component to active
        /// </summary>
        /// <param name="transform"></param>
        public static void Show(this MonoBehaviour transform)
        {
            transform.gameObject.SetActive(true);
        }

        /// <summary>
        /// Set the GameObject of the target Transform component to inactive
        /// </summary>
        /// <param name="transform"></param>
        public static void Hide(this MonoBehaviour transform)
        {
            transform.gameObject.SetActive(false);
        }
    }

}