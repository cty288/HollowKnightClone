using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public class Timer {
        public float CurrentSeconds;

        private float startSeconds;

        /// <summary>
        /// Start the timer
        /// </summary>
        public void StartTimer() {
            startSeconds = Time.time;
        }

        /// <summary>
        /// Calculate the time elapsed since the start of the timer
        /// </summary>
        public void UpdateTimeData() {
            CurrentSeconds = Time.time - startSeconds;
        }
    }
}
