using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MikroFramework
{
    public class HotUpdateExample : MonoBehaviour
    {
        public Text downloadText;
        void Start() {
            HotUpdateManager.Singleton.EnableUpdateDownloadSpeed(0.05f);
            StartCoroutine(HotUpdateSimpleTest(LoadAssets));
        }


        void Update() {
            downloadText.text = $"Total downloaded progress: {HotUpdateManager.Singleton.Downloader.GetDownloadProgress()}" +
                                $"\n Total downloading size: {HotUpdateManager.Singleton.Downloader.GetTotalDownloadFileSize()}" +
                                $"\n Already downloaded size: {HotUpdateManager.Singleton.Downloader.GetAlreadyDownloadedFileSize()}" +
                                $"\n Downloading file size: {HotUpdateManager.Singleton.Downloader.GetDownloadingFileSize()}" +
                                $"\n Download speed (kb/s): {HotUpdateManager.Singleton.Downloader.GetDownloadSpeed()}";
        }

        private ResLoader resLoader;
        private void LoadAssets() {
           resLoader = new ResLoader();
            //resLoader.LoadSync<AssetBundle>("scene");

            //SceneManager.LoadSceneAsync("HotUpdateExampleScene");

           // resLoader.LoadAsync<Texture2D>("reskit/test", "Asteroid_1", (obj) => {
              //  Debug.Log(obj.name);
          //  });

            //resLoader.LoadAsync<GameObject>("mftest", "TestABObj", (obj) => {
            // Instantiate(obj);
            // });

             resLoader.LoadAsync<GameObject>("mftest2", "TestABObj", (obj) => {
              Instantiate(obj);
             });


             resLoader.LoadAsync<GameObject>("mftest", "Cylinder", (obj) => {
                 Instantiate(obj);
             });

        }
        

        public IEnumerator HotUpdateSimpleTest(Action onHotUpdateFinished)
        {
            //Application.OpenURL(Application.persistentDataPath);
            bool finished = false;


            HotUpdateManager.Singleton.Init(() => {
                HotUpdateManager.Singleton.HasNewVersionRes((needUpdate, resVersion) => {
                    if (needUpdate)
                    {
                        HotUpdateManager.Singleton.UpdateRes(() => {
                            Debug.Log("Continue");
                           
                            //check complete
                            HotUpdateManager.Singleton.ValidateHotUpdateCompleteness((result) => {
                                HotUpdateManager.Singleton.DeleteRedundantFiles(() => {
                                    finished = true;
                                }, OnError);
                            }, OnError);
                        }, OnError);
                    }
                    else
                    {
                        //check complete
                        Debug.Log("Do not need hot-update. Continue");
                        HotUpdateManager.Singleton.ValidateHotUpdateCompleteness((result) => {
                            HotUpdateManager.Singleton.DeleteRedundantFiles(() => {
                                finished = true;
                            },OnError);
                        }, OnError);
                        
                    }
                }, OnError);


            }, OnError);

            while (!finished)
            {
                yield return null;
            }
            onHotUpdateFinished.Invoke();
        }

        private void OnError(HotUpdateError error) {
            Debug.Log(error.ToString());
        }

        private void OnDestroy() {
            resLoader.ReleaseAllAssets();
        }
    }


}
