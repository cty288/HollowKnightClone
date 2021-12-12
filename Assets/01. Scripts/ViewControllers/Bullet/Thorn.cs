using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public class Thorn : AbstractMikroController<HollowKnight> {
        public Vector2 TargetPos;
        public bool ShootInstant = true;
        public List<GameObject> attackedEnemy;
        private Animator animator;

        [SerializeField] private AudioClip shootClip;

        public int Damage;

        [SerializeField] private float shootSpeed = 30f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            attackedEnemy = new List<GameObject>();
            //this.RegisterEvent<OnSmallAnimalChargeReleased>(OnChargeReleased).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start() {
            Aim();
            AudioManager.Singleton.OnThornShoot(shootClip, 1);
        }

     
        void Aim()
        {
            Vector2 resultVec = TargetPos - new Vector2(transform.position.x, transform.position.y);

            float angle = Mathf.Atan2(resultVec.y, resultVec.x) * 180 / Mathf.PI;

            transform.DORotate(new Vector3(0, 0, angle), 0);
        }

      
       
        private void OnTriggerEnter2D(Collider2D other)
        {
           
            if (other.gameObject)
            {
                
                if (other.gameObject.TryGetComponent<IEnemyViewControllerAttackable>(out IEnemyViewControllerAttackable attackable))
                {
                    if (attackedEnemy.Contains(other.gameObject)) {
                        return;
                    }
                    attackedEnemy.Add(other.gameObject);
                    Debug.Log($"Thorn shoot an attackable {attackable.Attackable.Health.Value}," +
                              $"with damage: {Damage}");
                    this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.3f, 0.5f,
                        20, 100));
                    this.SendCommand<HurtEnemyCommand>(HurtEnemyCommand.Allocate(attackable.Attackable, Damage));
                }
            }
        }

        public void OnAnimatonEnds() {
            Destroy(this.gameObject);
        }

    }
}
