using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class TeleportArrowSpawnerViewController : AbstractMikroController<HollowKnight> {
        [SerializeField] 
        private GameObject arrowPrefab;

        [SerializeField] 
        private float arrowSpeed = 3f;

        private void Awake() {
            this.RegisterEvent<OnTeleportStart>(OnTeleportStart).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnTeleportStart(OnTeleportStart e) {
            GameObject arrow = Instantiate(arrowPrefab, transform);

            float time = Mathf.Abs(Vector2.Distance(transform.position, e.targetDest)) / arrowSpeed;

            float angle = Vector2.Angle(transform.position, e.targetDest);
            Debug.Log(angle);
        }
    }
}
