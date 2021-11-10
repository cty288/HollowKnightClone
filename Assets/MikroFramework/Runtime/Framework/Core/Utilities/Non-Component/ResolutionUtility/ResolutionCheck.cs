using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace MikroFramework.Utilities {
    public partial class ResolutionCheck : MonoBehaviour
    {

        /// <summary>
        /// Is the current device landscape?
        /// </summary>
        public static bool IsLandscape {
            get {
                return Screen.width > Screen.height;
            }
        }


        /// <summary>
        /// Get the Aspect Ratio of the current device
        /// </summary>
        /// <returns>aspect ratio</returns>
        public static float GetAspectRatio() {
            var isLandscape = IsLandscape;
            return isLandscape? (float)Screen.width / Screen.height: (float)Screen.height / Screen.width;
        }

        /// <summary>
        /// Return if the current resolution is IPad's resolution(4:3)
        /// </summary>
        /// <returns></returns>
        public static bool IsPadResolution4_3() {
            return InAspectRange(4.0f / 3);
        }

        /// <summary>
        /// Return if the current resolution is IPhone's (excluding IPhone 4S and IPhone X+) resolution (or 16:9).
        /// Including iPhone 5/5S/5C/SE, iPhone 6/6S/7/8, iPhone 6/6S/7/8 Plus
        /// </summary>
        /// <returns></returns>
        public static bool IsIPhoneResolution16_9() {
            return InAspectRange(16.0f / 9);
        }

        /// <summary>
        /// Returns if the current resolution is Iphone 4S resolution
        /// </summary>
        /// <returns></returns>
        public static bool IsIPhone4SResolution() {
            return InAspectRange(3f / 2);
        }

        /// <summary>
        /// Return if the current resolution is IPhone X resolution
        /// </summary>
        /// <returns></returns>
        public static bool IsIphoneXResolution() {
            return InAspectRange(2436.0f / 1125);
        }

        /// <summary>
        /// Return if the current resolution is IPhone XR or 11 resolution
        /// </summary>
        /// <returns></returns>
        public static bool IsIphoneXR11Resolution()
        {
            return InAspectRange(1792.0f / 828);
        }

        /// <summary>
        /// Return if the current resolution is IPhone XS or 11 Pro resolution
        /// </summary>
        /// <returns></returns>
        public static bool IsIphoneXS11PResolution()
        {
            return InAspectRange(2436.0f / 1125);
        }


        /// <summary>
        /// Return if the current resolution is IPhone XS Max or 11 Pro Max resolution
        /// </summary>
        /// <returns></returns>
        public static bool IsIphoneXSMax11PMaxResolution()
        {
            return InAspectRange(2688.0f / 1242);
        }

        /// <summary>
        /// Check if the current device's aspect ratio is in the range of the target aspect ratio
        /// </summary>
        /// <param name="targetAspectRatio">The target aspect ratio to check</param>
        /// <returns></returns>
        public static bool InAspectRange(float targetAspectRatio) {
            var aspect = GetAspectRatio();
            return aspect > (targetAspectRatio - 0.05) && aspect < (targetAspectRatio + 0.05);
        }

    }
}

