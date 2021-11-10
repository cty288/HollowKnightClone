using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

namespace HollowKnight
{
    public class MapPlayer : MonoBehaviour, ISingleton {
        private Player player;

        private void Awake() {
            player = Player.Singleton;
        }

        private void FixedUpdate() {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y,
                transform.position.z);
        }

        public void OnSingletonInit() {
            
        }
    }
}
