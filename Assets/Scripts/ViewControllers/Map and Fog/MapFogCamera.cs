using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class MapFogCamera : AbstractMikroController<HollowKnight> {
        [SerializeField] private GameObject fogPlane;
        [SerializeField] private Transform mapPlayer;
        [SerializeField] private LayerMask targetLayer;
       

        private Mesh mesh;
        private Vector3[] vertices;
        

      
        private void Update() {
            Ray ray = new Ray(transform.position, mapPlayer.position - transform.position);

            if (Physics.Raycast(ray, out RaycastHit info, 2000, targetLayer, QueryTriggerInteraction.Collide)) {
                this.GetSystem<IMapSystem>().OnCameraRaycastHit(info);
            }

        }
    }
}
