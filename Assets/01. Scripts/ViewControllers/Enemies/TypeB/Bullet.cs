using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject target;
        private Vector3 shootDir;

        public void SetDir (Vector3 shootDir)
        {
            this.shootDir = shootDir;
            transform.eulerAngles = new Vector3(0, 0, floatVectorToAngle(shootDir));
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
    }
}
