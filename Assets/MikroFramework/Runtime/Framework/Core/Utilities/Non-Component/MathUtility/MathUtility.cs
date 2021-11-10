using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MikroFramework.Utilities {
    public partial class MathUtility {
        /// <summary>
        /// Given the possibility, return whether the random result hit this possibility
        /// </summary>
        /// <param name="percent">Possibility in percent (0~100)</param>
        /// <returns></returns>
        public static bool Percent(int percent)
        {
            return Random.Range(0, 100) < percent;
        }

        /// <summary>
        /// Get a random element from an array of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T GetRandomValueFrom<T>(params T[] values)
        {
            return values[Random.Range(0, values.Length)];
        }
    }
}

