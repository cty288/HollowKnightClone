using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using UnityEngine;

namespace HollowKnight {
    public class Player : AbstractMikroController<HollowKnight>, ISingleton {
        public static Player Singleton {
            get {
                return SingletonProperty<Player>.Singleton;
            }
        }
        private Rigidbody2D rigidbody2D;

        [SerializeField] private float moveSpeed =5f;

        private void Awake() {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            if (horizontalMovement > 0 && transform.localScale.x < 0 ||
                horizontalMovement < 0 && transform.localScale.x > 0)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
            }

           
                rigidbody2D.velocity = new Vector2(horizontalMovement, verticalMovement).normalized * moveSpeed;
            
           
        }

        void ISingleton.OnSingletonInit() {
            
        }
    }

}
