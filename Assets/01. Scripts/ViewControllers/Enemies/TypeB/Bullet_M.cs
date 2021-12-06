using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HollowKnight
{
    public class Bullet_M : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject target;
        [SerializeField] private BoxCollider2D bc;
        private Vector3 shootDir;

        public void SetDir (Vector3 shootDir)
        {
            this.shootDir = shootDir;
            Vector2 resultVec = target.transform.position - new Vector3(transform.position.x, transform.position.y, 0);

            float angle = Mathf.Atan2(resultVec.y, resultVec.x) * 180 / Mathf.PI;


            transform.DORotate(new Vector3(0, 0, angle), 0);
        }

        private float floatVectorToAngle(Vector3 dir)
        {
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }

        private void Update()
        {
            transform.position += shootDir * Time.deltaTime * speed;
        }
        public void OnHitFinished()
        {
            Destroy(this.gameObject);
        }
    }
}
