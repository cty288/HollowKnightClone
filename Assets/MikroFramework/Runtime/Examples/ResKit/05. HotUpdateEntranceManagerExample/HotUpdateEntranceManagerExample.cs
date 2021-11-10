using System.Collections;
using System.Collections.Generic;
using MikroFramework.Managers;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.UI;

namespace MikroFramework.Examples
{
    public class HotUpdateEntranceManagerExample : HotUpdateEntranceManager
    {
        protected override void LaunchInDevelopingMode() {
            StartHotUpdate();
        }

        protected override void LaunchInTestingMode() {
            StartHotUpdate();
        }


        protected override void OnHotUpdateError(HotUpdateError error) {
            Debug.Log(error.ToString());
        }

        protected override void OnHotUpdateManagerInitialized() {
            
        }

        protected override void OnHotUpdateVersionChecked(bool needUpdate, ResVersion localResVersion) {
            if (needUpdate) {
                Debug.Log("Need update");
            }
            else {
                Debug.Log("Do not need update");
            }
        }

        public Text downloadText;
        protected override void OnHotUpdateResourcesDownloading(float downloadProgress, float totalDownloadSize, float alreadyDownloadedFileSize,
            float downloadingFileDownloadedSize, float downloadSpeed) {
            downloadText.text = $"Total downloaded progress: {HotUpdateManager.Singleton.Downloader.GetDownloadProgress()}" +
                                $"\n Total downloading size: {HotUpdateManager.Singleton.Downloader.GetTotalDownloadFileSize()}" +
                                $"\n Already downloaded size: {HotUpdateManager.Singleton.Downloader.GetAlreadyDownloadedFileSize()}" +
                                $"\n Downloading file size: {HotUpdateManager.Singleton.Downloader.GetDownloadingFileSize()}" +
                                $"\n Download speed (kb/s): {HotUpdateManager.Singleton.Downloader. GetDownloadSpeed()}";
        }

        protected override void OnHotUpdateResourceDownloadedAndUpdated() {
            
        }

        protected override void OnResourceCompletenessValidated(List<ABMD5Base> updatedResourceInfos) {
            downloadText.text = "";
        }

        protected override void OnRedundantFilesDeleted() {

        }

        protected override void OnHotUpdateComplete() {
            ResLoader resLoader = new ResLoader();

            resLoader.LoadAsync<GameObject>("mftest2", "TestABObj", (obj) => {
                Instantiate(obj);
            });


            resLoader.LoadAsync<GameObject>("mftest", "Cylinder", (obj) => {
                Instantiate(obj);
            });
        }
    }
}
