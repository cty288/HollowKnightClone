using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Utilities
{
    public class TriggerCheck : MonoBehaviour {
        public LayerMask TargetLayers;

        
        private SimpleRC enterRC = new SimpleRC();

        [SerializeField]
        private List<Collider> colliders;
        /// <summary>
        /// Get all colliders that are in the current trigger of this object
        /// </summary>
        public List<Collider> Colliders => colliders;


        /// <summary>
        /// If there are any collider in the trigger of this object
        /// </summary>
        public bool Triggered
        {
            get { return enterRC.RefCount > 0; }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (PhysicsUtility.IsInLayerMask(other.gameObject, TargetLayers))
            {
                enterRC.Retain();
                colliders.Add(other);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (PhysicsUtility.IsInLayerMask(other.gameObject, TargetLayers))
            {
                enterRC.Release();
                colliders.Remove(other);
            }

        }
    }
}
