using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace MikroFramework.Extensions {
    public static partial class TransformExtension
    {
        /// <summary>
        /// Reset the Transform component
        /// </summary>
        /// <param name="transform">Transform component to reset</param>
        public static void Identity(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Reset the Transform component
        /// </summary>
        /// <param name="transform">Transform component to reset</param>
        public static void Identity(this MonoBehaviour monoBehaviour)
        {
            monoBehaviour. transform.localPosition = Vector3.zero;
            monoBehaviour.transform.localScale = Vector3.one;
            monoBehaviour.transform.localRotation = Quaternion.identity;
        }
        public static void SetLocalPosX(this Transform transform, float x)
        {
            var localPosition = transform.localPosition;
            localPosition.x = x;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosY(this Transform transform, float y)
        {
            var localPosition = transform.localPosition;
            localPosition.y = y;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosZ(this Transform transform, float z)
        {
            var localPosition = transform.localPosition;
            localPosition.z = z;
            transform.localPosition = localPosition;
        }

        public static void SetLocalPosXY(this Transform transform, float x, float y)
        {
            var localPos = transform.localPosition;
            localPos.x = x;
            localPos.y = y;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosXZ(this Transform transform, float x, float z)
        {
            var localPos = transform.localPosition;
            localPos.x = x;
            localPos.z = z;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosYZ(this Transform transform, float y, float z)
        {
            var localPos = transform.localPosition;
            localPos.y = y;
            localPos.z = z;
            transform.localPosition = localPos;
        }

        /// <summary>
        /// Add a gameobject to the child of another gameobject
        /// </summary>
        /// <param name="parentTransform">The Transform component of the parent GameObject</param>
        /// <param name="childTransform">The Transform component of the child GameObject</param>
        public static void AddChild(this Transform parentTransform, Transform childTransform)
        {
            childTransform.SetParent(parentTransform);
        }

        public static void SetPosX(this Transform transform, float x)
        {
            var position = transform.position;
            position.x = x;
            transform.position = position;
        }

        public static void SetPosY(this Transform transform, float y)
        {
            var position = transform.position;
            position.y = y;
            transform.position = position;
        }

        public static void SetPosZ(this Transform transform, float z)
        {
            var position = transform.position;
            position.z = z;
            transform.position = position;
        }
        public static void SetPosXY(this Transform transform, float x, float y)
        {
            var position = transform.position;
            position.x = x;
            position.y = y;
            transform.position = position;
        }

        public static void SetPosXZ(this Transform transform, float x, float z)
        {
            var position = transform.position;
            position.x = x;
            position.z = z;
            transform.position = position;
        }

        public static void SetPosYZ(this Transform transform, float y, float z)
        {
            var position = transform.position;
            position.y = y;
            position.z = z;
            transform.position = position;
        }
    }


}
