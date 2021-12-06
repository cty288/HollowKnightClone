using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MikroFramework.Architecture;

namespace HollowKnight
{
    public class Bullet_M : AbstractMikroController<HollowKnight>
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private Rigidbody2D rb;
       private GameObject target;
        [SerializeField] private BoxCollider2D bc;
        private Vector3 shootDir;

        [SerializeField] 
        private int damage = 5;

        private Animator animator;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Start() {
            target = Player.Singleton.gameObject;
            SetDir();
            
        }

        public void SetDir ()
        {
            Vector2 resultVec = target.transform.position - new Vector3(transform.position.x, transform.position.y, 0);

            float angle = Mathf.Atan2(resultVec.y, resultVec.x) * 180 / Mathf.PI;

            float yAngle = target.transform.position.x - transform.position.x > 0 ? 0 : -180;
            
            Debug.Log(yAngle);
            transform.DORotate(new Vector3(0, 0, angle), 0);
        }

        private float floatVectorToAngle(Vector3 dir)
        {
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }

        private void FixedUpdate()
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }
        public void OnHitFinished()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject)
            {
                if (other.gameObject != this.gameObject && !other.isTrigger) {
                    speed = 0;
                    animator.SetTrigger("Hit");
                    Debug.Log(other.gameObject.name);
                    if (other.gameObject == Player.Singleton.gameObject)
                    {
                        this.GetModel<IPlayerModel>().ChangeHealth(-damage);
                    }
                }
                
        }
        }

        }
}

