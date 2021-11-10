using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Utilities
{
    public static class RectHelper {
        
        /// <summary>
        /// Return the rect, which is anchored at its center, given its x, y, width and height (anchored at center)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Rect RectForAnchorCenter(float x, float y, float width, float height) {
            float newX = x - width * 0.5f;
            float newY = y - height * 0.5f;
            return new Rect(newX, newY, width, height);
        }
        /// <summary>
        /// Return the rect, which is anchored at its center, given its pos and size (anchored at center)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Rect RectForAnchorCenter(Vector2 pos, Vector2 size)
        {
            float newX = pos.x - size.x* 0.5f;
            float newY = pos.y - size.y * 0.5f;
            return new Rect(newX, newY, size.x, size.y);
        }
    }
}
