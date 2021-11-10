using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Utilities
{
    public static class InputUtility {
        /// <summary>
        /// Return the move direction and length of the mouse in the current frame
        /// </summary>
        /// <param name="sensitivity">Sensitivity of the mouse</param>
        /// <returns></returns>
        public static Vector2 GetMouseMoveDirection(float sensitivity=1) {
            Vector2 direction = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            return direction * sensitivity;
        }
    }
}
