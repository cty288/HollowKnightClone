using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Utilities
{
    public class Trigger2DCheck : MonoBehaviour {
        public LayerMask TargetLayers;



        private SimpleRC enterRC = new SimpleRC();

        [SerializeField]
        private List<Collider2D> colliders;
        /// <summary>
        /// Get all 2D colliders that are in the current trigger of this object
        /// </summary>
        public List<Collider2D> Colliders => colliders;

        /// <summary>
        /// If there are any collider in the trigger of this object
        /// </summary>
        public bool Triggered
        {
            get { return enterRC.RefCount > 0; }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (PhysicsUtility.IsInLayerMask(other.gameObject, TargetLayers)) {
                enterRC.Retain();
                colliders.Add(other);
            }

        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (PhysicsUtility.IsInLayerMask(other.gameObject, TargetLayers)) {
                enterRC.Release();
                colliders.Remove(other);
            }
           
        }

    }
}
