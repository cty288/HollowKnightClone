using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public class MapPlane : AbstractMikroController<HollowKnight>
    {
        [SerializeField] private float radius = 5;
        public float Radius => radius;

        private Mesh mesh;
        private Vector3[] vertices;
        public Vector3[] Vertices => vertices;

        private Color[] colors;
        public Color[] Colors => colors;

        private void Awake() {
            mesh = GetComponent<MeshFilter>().mesh;
            vertices = mesh.vertices;
            colors = new Color[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                colors[i] = Color.black;
                vertices[i] = transform.TransformPoint(vertices[i]);
            }

            mesh.colors = colors;
        }

        private void Start() {
            this.RegisterEvent<OnMapMeshRender>(OnMapMeshRendered).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnMapMeshRendered(OnMapMeshRender e) {
            Debug.Log("Mesh rendering");
            mesh.colors = e.meshColors;
        }

    }
}
