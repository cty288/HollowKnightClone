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
        private float arrowSpeed = 20f;

        private void Awake() {
            this.RegisterEvent<OnTeleportStart>(OnTeleportStart).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnTeleportStart(OnTeleportStart e) {
            GameObject arrow = Instantiate(arrowPrefab, transform);

            float time = Mathf.Abs(Vector2.Distance(transform.position, e.targetDest)) / arrowSpeed;

            Vector2 resultVec = e.targetDest - new Vector2(transform.position.x, transform.position.y);
            float angle = Mathf.Atan2(Mathf.Abs(resultVec.y), Mathf.Abs(resultVec.x)) * 180 / Mathf.PI;
            Debug.Log(time);

            Vector3 localRotation = arrow.transform.localRotation.eulerAngles;

            if (e.targetDest.y >= transform.position.y) {
                arrow.transform.DOLocalRotate(localRotation + new Vector3(0, 0, angle),0);
            }
            else {
                arrow.transform.DOLocalRotate(localRotation - new Vector3(0, 0, angle), 0);
            }

            arrow.GetComponent<TeleportArrowViewController>().Target = e.targetDest;
            //arrow.transform.SetParent(null);
            arrow.transform.DOMove(e.targetDest, time).SetEase(Ease.Linear);

        }

        IEnumerator ChangeArrowParent(Transform tr) {
            yield return new WaitForSeconds(0.1f);
            tr.SetParent(null);
        }
    }
}
