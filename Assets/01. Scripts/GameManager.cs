using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using UnityEngine;

namespace HollowKnight
{
    public struct OnPlayerRespawned {

    }
    public class GameManager : AbstractMikroController<HollowKnight>, ISingleton, ICanSendEvent {
        public bool DaggerGet = false;

        private int respawnPoint = -1;

        [SerializeField] private List<Transform> respawnPositions;

        [SerializeField] private GameObject playerPrefab;

        public static GameManager Singleton {
            get {
                return SingletonProperty<GameManager>.Singleton;
            }
        }

        public void RespawnPointRecord(int inputRespawnPoint) {
            if (inputRespawnPoint > respawnPoint) {
                respawnPoint = inputRespawnPoint;
            }
        }

        public void Respawn() {
            //Destroy(Player.Singleton.gameObject);
            Player.Singleton.transform.position = respawnPositions[respawnPoint].position;
            this.SendEvent<OnPlayerRespawned>(new OnPlayerRespawned());
            //Instantiate(playerPrefab, respawnPositions[respawnPoint].position, Quaternion.identity);
        }

        private void Awake() {
            this.RegisterEvent<OnDaggerGet>(OnDaggerGet).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

         void ISingleton.OnSingletonInit() {
            
        }

        private void OnDaggerGet(OnDaggerGet eGet) {
            DaggerGet = true;
            Debug.Log("Dagger get");
        }
    }
}
