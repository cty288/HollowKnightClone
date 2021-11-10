using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class MapFogCamera : MonoBehaviour {
        [SerializeField] private GameObject fogPlane;
        [SerializeField] private Transform mapPlayer;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float radius = 5;

        private Mesh mesh;
        private Vector3[] vertices;
        private Color[] colors;

        private void Awake() {
            mesh = fogPlane. GetComponent<MeshFilter>().mesh;
            vertices = mesh.vertices;
            colors = new Color[vertices.Length];

            for (int i = 0; i < vertices.Length; i++) {
                colors[i] = Color.black;
                vertices[i] = fogPlane.transform.TransformPoint(vertices[i]);
            }

            mesh.colors = colors;
        }

        private void Update() {
            Ray ray = new Ray(transform.position, mapPlayer.position - transform.position);

            if (Physics.Raycast(ray, out RaycastHit info, 2000, targetLayer, QueryTriggerInteraction.Collide)) {
               Debug.Log(info.collider.gameObject.name);
                for (int i = 0; i < vertices.Length; i++) {
                    float distance = Vector3.SqrMagnitude(vertices[i] - info.point);
                    if (distance < radius * radius) {
                        float alpha = Mathf.Min(colors[i].a, distance / (radius * radius));
                        colors[i].a = alpha;
                    }
                }
            }

            mesh.colors = colors;
        }
    }
}
