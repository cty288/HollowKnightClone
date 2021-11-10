using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Utilities
{
    public interface IRefCounter {
        /// <summary>
        /// The count of the current reference counter
        /// </summary>
        int RefCount { get; }

        /// <summary>
        /// Let the current RC+1
        /// </summary>
        /// <param name="refOwner"></param>
        void Retain(object refOwner = null);

        /// <summary>
        /// Let the current RC-1.
        /// </summary>
        /// <param name="refOwner"></param>
        void Release(object refOwner = null);
    }
    public class SimpleRC : IRefCounter
    {
        public int RefCount { get; private set; }

        /// <summary>
        /// Let the current RC-1. If RC=0, trigger OnZeroRef function
        /// </summary>
        /// <param name="refOwner"></param>
        public void Release(object refOwner = null)
        {
            RefCount--;
            if (RefCount == 0)
            {
                OnZeroRef();
            }

        }


        public void Retain(object refOwner = null) {
            RefCount++;
        }


        /// <summary>
        /// Triggered when the reference counter reaches 0
        /// </summary>
        protected virtual void OnZeroRef() {

        }
    }
}
