using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Singletons;
using MikroFramework.TimeSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HollowKnight
{
    public enum GameState {
        Menu,
        Game,
        End
    }
    public struct OnPlayerRespawned { 

    }

    public struct OnGameStart {

    }
    public struct OnSceneSwitch {
        public int TargetScene;
    }

    public struct OnGameEnd {

    }
    public class GameManager : AbstractMikroController<HollowKnight>, ISingleton, ICanSendEvent {
        
        public bool DaggerGet = false;
        private GameState state = GameState.Menu;
        public GameState State {
            get { return state; }
        }

        private int respawnPoint = -1;

        [SerializeField] private List<Transform> respawnPositions;

        [SerializeField] private GameObject playerPrefab;

        [SerializeField] private GameObject[] scenes;

        [SerializeField] private GameObject churchChargeEnemy;

        public bool CanLeaveChurch = false;
        public static GameManager Singleton;

        private int sceneCount = 0;
        
        

        private void Update() {
            if (state == GameState.Menu) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    this.SendEvent<OnGameStart>();
                    state = GameState.Game;
                }
            }

            if (state == GameState.End) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    this.SendEvent<OnGameEnd>(new OnGameEnd());
                    state = GameState.Menu;
                    respawnPoint = -1;
                    DaggerGet = false;
                    CanLeaveChurch = false;

                    for (int i = 0; i < scenes.Length; i++)
                    {
                        if (i != sceneCount)
                        {
                            scenes[i].SetActive(false);
                        }
                    }
                }
            }
        }

        public void SwitchScene(int sceneNum) {
           
            this.SendEvent<OnSceneSwitch>(new OnSceneSwitch(){TargetScene = sceneNum});
            this.GetSystem<ITimeSystem>().AddDelayTask(1, () => {
                scenes[sceneCount].SetActive(false);
                sceneCount = sceneNum;
                scenes[sceneNum].SetActive(true);
            });
            
        }

        public void EndGame() {
            state = GameState.End;
            
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
            DontDestroyOnLoad(this.gameObject);
            if (Singleton != null) {
                Destroy(this.gameObject);
            }
            else {
                Singleton = this;
            }
            this.RegisterEvent<OnDaggerGet>(OnDaggerGet).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start() {
            for (int i = 0; i< scenes.Length; i++) {
                if (i != sceneCount) {
                    scenes[i].SetActive(false);
                }
            }
        }

        void ISingleton.OnSingletonInit() {
            
        }

        public void ChurchChargeEnemyActivate() {
            if (DaggerGet && churchChargeEnemy) {
                churchChargeEnemy.SetActive(true);
            }
        }
        private void OnDaggerGet(OnDaggerGet eGet) {
            DaggerGet = true;
            CanLeaveChurch = true;
            Debug.Log("Dagger get");
        }
    }
}
