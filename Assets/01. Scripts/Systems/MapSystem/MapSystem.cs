using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    
    public interface IMapSystem : ISystem {
        void RenderMap();
        void OnCameraRaycastHit(RaycastHit hitInfo);
    }

    public struct OnMapMeshRender {
        public Color[] meshColors;
    }

    public class MapSystem : AbstractSystem, IMapSystem
    {
        private MapPlane fogPlane;
        private Vector3[] vertices;
        private Color[] colors;

        protected override void OnInit() {
            fogPlane = GameObject.Find("MapFogPlane").GetComponent<MapPlane>();
            vertices = fogPlane.Vertices;
            colors = fogPlane.Colors;
        }

        public void OnCameraRaycastHit(RaycastHit hitInfo) {
            float radius = fogPlane.Radius;
            for (int i = 0; i < vertices.Length; i++)
            {
                float distance = Vector3.SqrMagnitude(vertices[i] - hitInfo.point);
                if (distance < radius * radius)
                {
                    float alpha = Mathf.Min(colors[i].a, distance / (radius * radius));
                    colors[i].a = alpha;
                }
            }
        }

        public void RenderMap() {
            this.SendEvent<OnMapMeshRender>(new OnMapMeshRender(){meshColors = colors});
        }
    }
}
