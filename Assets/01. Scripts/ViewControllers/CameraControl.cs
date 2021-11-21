using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class CameraControl : MonoBehaviour {
        private Player player;
        private Camera camera;

        [SerializeField]
        private float lerpSpeed = 20;

        [SerializeField] private Vector2 cameraPositionXRange = new Vector2(0, 100);
        [SerializeField] private Vector2 cameraPositionYRange = new Vector2(0, 100);

       
        private void Awake() {
            camera = GetComponent<Camera>();
            player = Player.Singleton;
        }

        private void Update() {
            float targetX = transform.position.x;
            targetX = Mathf.Lerp(targetX, player.transform.position.x, lerpSpeed * Time.deltaTime);
            targetX = Mathf.Clamp(targetX, cameraPositionXRange.x, cameraPositionXRange.y);

            float targetY = transform.position.y;
            targetY = Mathf.Lerp(targetY, player.transform.position.y, lerpSpeed * Time.deltaTime);
            targetY = Mathf.Clamp(targetY, cameraPositionYRange.x, cameraPositionYRange.y);

            transform.position = new Vector3(targetX, targetY, transform.position.z);
        }
    }
}
