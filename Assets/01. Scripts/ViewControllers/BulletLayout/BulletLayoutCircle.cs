using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public class BulletLayoutCircle : AbstractMikroController<HollowKnight> {
        
        private Rigidbody2D playerRb;

        [SerializeField] private float lerp = 0.1f;

        private float targetLerp;

        private CircleLayoutGroup circleLayoutGroup;
        private float targetValue = 90;
        private float prevTarget = 90;
        private void Awake() {
            
           
            targetLerp = lerp;
            circleLayoutGroup = GetComponentInChildren<CircleLayoutGroup>();
        }

        private void Start() {
            this.RegisterEvent<OnAbsorbableEnemyViewControllerAddedToLayoutCircle>(OnEnemyAdded).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnWeaponShifted>(OnWeaponShifted).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnWeaponDropped>(OnWeaponDropped).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnPlayerRespawned>(OnRespawn).UnRegisterWhenGameObjectDestroyed(gameObject);
            for (int i = 0; i < circleLayoutGroup.transform.childCount; i++)
            {
                Transform cur = circleLayoutGroup.transform.GetChild(i);
                cur.DOScaleX(-1, 0f);
            }
        }

        private void OnRespawn(OnPlayerRespawned obj) {
            for (int i = 0; i < circleLayoutGroup.transform.childCount; i++)
            {
                Destroy(circleLayoutGroup.transform.GetChild(i).gameObject);
                
            }
        }

        private void OnWeaponDropped(OnWeaponDropped obj) {
            //StartCoroutine(ChangeSiblingOrder());
        }

        private void OnWeaponShifted(OnWeaponShifted e) {
            if (targetValue == 90) {
                if (e.Up) {
                    MoveLastToFirst();
                }
                else {
                    MoveFirstToLast();
                }
            }
            else {
                if (e.Up) {
                    MoveFirstToLast();
                }
                else {
                    MoveLastToFirst();
                }
            }
        }


        private void MoveLastToFirst() {
            Transform circleLayoutTr = circleLayoutGroup.transform;
            circleLayoutTr.GetChild(circleLayoutTr.childCount-1).SetAsFirstSibling();
        }

        private void MoveFirstToLast() {
            Transform circleLayoutTr = circleLayoutGroup.transform;
            circleLayoutTr.GetChild(0).SetAsLastSibling();
        }

        private void OnEnemyAdded(OnAbsorbableEnemyViewControllerAddedToLayoutCircle e) {
            Transform parentTr = e.viewControllerGameObject.transform;
            parentTr.SetParent(circleLayoutGroup.transform);
            parentTr.rotation = Quaternion.identity;
            

            if (targetValue == 90) {
                parentTr.SetAsFirstSibling();
            }

            if (targetValue == 0) {
                parentTr.SetAsLastSibling();
            }
           
        }

        private void FixedUpdate() {
            UpdateLerpSpeed();
            FollowPlayer();
           UpdateRotation();
        }

        private void UpdateRotation() {
            
            if (Player.Singleton.FaceRight) {
                targetValue = 90;

                for (int i = 0; i < circleLayoutGroup.transform.childCount; i++) {
                    Transform cur = circleLayoutGroup.transform.GetChild(i);
                    if (cur) {
                        cur.DOScaleX(-1, 0f);
                    }
                    
                }
            }

            if (!Player.Singleton.FaceRight) {
                for (int i = 0; i < circleLayoutGroup.transform.childCount; i++)
                {
                    Transform cur = circleLayoutGroup.transform.GetChild(i);
                    if (cur) {
                        cur.DOScaleX(1, 0f);
                    }
                   
                }
                if (circleLayoutGroup.transform.childCount > 1) {
                    targetValue = 0;
                    
                }
                else {
                    targetValue = 90;
                }
               
            }

            if (prevTarget != targetValue) {

                StartCoroutine(ChangeSiblingOrder());
                prevTarget = targetValue;
            }
            
            DOTween.To(() => circleLayoutGroup.initAngle,
                x => circleLayoutGroup.initAngle = x,
                targetValue, 0.1f
            );
        }

        private IEnumerator ChangeSiblingOrder() {
            yield return new WaitForSeconds(0.08f);

            for (int i = 0; i < circleLayoutGroup.transform.childCount / 2; i++)
            {
                Transform top = circleLayoutGroup.transform.GetChild(i);
                int downIndex = circleLayoutGroup.transform.childCount - 1 - i;
                Transform down = circleLayoutGroup.transform.GetChild(downIndex);

                top.SetSiblingIndex(downIndex);
                down.SetSiblingIndex(i);
            }
        }

        private void UpdateLerpSpeed() {
            float speed = Player.Singleton.Speed.Value;
            if (speed <= 1 && speed >= 0)
            {
                targetLerp = lerp;
            }
            else
            {
                targetLerp = Mathf.Lerp(targetLerp, 1, 0.01f);

            }
        }

        private void FollowPlayer() {
            Vector3 playerPos = Player.Singleton.transform.position;
            this.transform.position = Vector3.Lerp(transform.position, playerPos, targetLerp);
        }
    }
}
