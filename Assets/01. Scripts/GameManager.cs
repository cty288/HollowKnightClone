using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private List<GameObject> scenes;

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
                    sceneCount = 0;

                    StartCoroutine(ResetSceneDelay());
                }
            }
        }

        IEnumerator ResetSceneDelay() {
            yield return new WaitForSeconds(0.2f);
            UpdateEnvironmentListAndRespawnPoint();
            UpdateEnvironment();
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
            UpdateEnvironmentListAndRespawnPoint();
            UpdateEnvironment();
        }

        public void UpdateEnvironmentListAndRespawnPoint() {
            scenes.Clear();
            respawnPositions.Clear();
            churchChargeEnemy = GameObject.Find("ChurchChargeEnemy");
            churchChargeEnemy.gameObject.SetActive(false);
            int i = 1;
            while (GameObject.Find($"Environment{i.ToString()}")) {
                scenes.Add(GameObject.Find($"Environment{i.ToString()}"));
                i++;
            }

            i = 1;

            while (GameObject.Find($"RespawnPointRecord{i.ToString()}"))
            {
                respawnPositions.Add(GameObject.Find($"RespawnPointRecord{i.ToString()}").transform);
                i++;
            }
        }

        public void UpdateEnvironment() {
            for (int i = 0; i < scenes.Count; i++) {
                if (i != sceneCount) {
                    scenes[i].SetActive(false);
                }
                else {
                    scenes[i].SetActive(true);
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
