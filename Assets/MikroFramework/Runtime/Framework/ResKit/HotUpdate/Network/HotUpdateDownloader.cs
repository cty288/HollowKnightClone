using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikroFramework.ResKit;
using MikroFramework.Serializer;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace MikroFramework.ResKit
{
    public class HotUpdateDownloader: MonoBehaviour, IHotUpdateDownloader
    {
        private List<ABMD5Base> totalDownloadingFiles = new List<ABMD5Base>();
        private List<ABMD5Base> filesAlreadyDownloaded = new List<ABMD5Base>();
        private UnityWebRequest downloadingFileRequest = null;

        private bool showDownloadSpeedEnabled = false;
        private float downloadSpeedUpdateTimeInterval = 1f;
        private float downloadSpeed;


        /// <summary>
        /// Get Local AssetBundle ResVersion from local AB folder. Return null if not found
        /// </summary>
        /// <param name="getResVersion"></param>
        /// <returns></returns>
        public IEnumerator GetLocalAssetResVersion(Action<ResVersion> getResVersion, Action<HotUpdateError>
            onGetFailed)
        {
            string localResVersionPath = HotUpdateConfig.LocalResVersionFilePath;

            UnityWebRequest request = UnityWebRequest.Get(localResVersionPath);

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                //getResVersion.Invoke(null);
                onGetFailed.Invoke(HotUpdateError.LocalResVersionNotExist);
            }
            else
            {
                string jsonString = request.downloadHandler.text;
                ResVersion resVersion = AdvancedJsonSerializer.Singleton.Deserialize<ResVersion>(jsonString);
                getResVersion.Invoke(resVersion);
            }
        }

        /// <summary>
        /// Get the ResVersion file from remote server
        /// </summary>
        /// <param name="onResDownloaded"></param>
        /// <returns></returns>
        public IEnumerator RequestGetRemoteResVersion(Action<ResVersion> onResDownloaded,
            Action<HotUpdateError> onError)
        {
            string remoteResVersionPath =HotUpdateConfig.RemoteResVersionURL;
            Debug.Log(remoteResVersionPath);

            UnityWebRequest request = UnityWebRequest.Get(remoteResVersionPath);
            yield return request.SendWebRequest();
            
            if (request.isHttpError || request.isNetworkError)
            {
                onError.Invoke(HotUpdateError.DownloadRemoeResVersionFailed);
            }
            else
            {
                string jsonString = request.downloadHandler.text;
                ResVersion resVersion = AdvancedJsonSerializer.Singleton.Deserialize<ResVersion>(jsonString);
                onResDownloaded.Invoke(resVersion);
            }
        }

        /// <summary>
        /// Download all resources from remote server.
        /// </summary>
        /// <param name="remoteResVersion"></param>
        /// <param name="onloadedDone"></param>
        /// <returns></returns>
        public void DoDownloadResVersion(ResVersion remoteResVersion)
        {
            if (!Directory.Exists(ResKitUtility.TempAssetBundlesPath))
            {
                Directory.CreateDirectory(ResKitUtility.TempAssetBundlesPath);
            }

            string tempResVersionFilePath = ResKitUtility.TempAssetBundlesPath + "ResVersion.json";
            string tempResVersionJson = AdvancedJsonSerializer.Singleton.Serialize(remoteResVersion);
            File.WriteAllText(tempResVersionFilePath, tempResVersionJson);
        }


        /// <summary>
        /// Download Asset Bundles from the remote server to the temp folder, given a list of ABMD5 infos.
        /// </summary>
        /// <param name="downloadList"></param>
        /// <param name="onDownloadDone">Event triggered after download. If download failed, the bool parameter will be false</param>
        /// <returns></returns>
        public IEnumerator DoDownloadRemoteABs(List<ABMD5Base> downloadList,
            Action<List<ABMD5Base>> onDownloadDone, Action<HotUpdateError> onDownloadFailed)
        {

            filesAlreadyDownloaded.Clear();
            totalDownloadingFiles = downloadList;

            string remoteBasePath =HotUpdateConfig.RemoteAssetBundleBaseURL;

            foreach (ABMD5Base downloadAsset in downloadList)
            {
                downloadingFileRequest = UnityWebRequest.Get(remoteBasePath + downloadAsset.AssetName);

                
                yield return downloadingFileRequest.SendWebRequest();

                if (downloadingFileRequest.isHttpError || downloadingFileRequest.isNetworkError)
                {
                    Debug.Log(downloadingFileRequest.error);
                    onDownloadFailed.Invoke(HotUpdateError.DownloadRemoteABFailed);
                    break;
                }
                else
                {
                    filesAlreadyDownloaded.Add(downloadAsset);
                    byte[] bytes = downloadingFileRequest.downloadHandler.data;
                    string filePath = ResKitUtility.TempAssetBundlesPath + downloadAsset.AssetName;

                    string directory = filePath.Substring(0, filePath.LastIndexOf('/'));

                    Debug.Log(directory);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.WriteAllBytes(filePath, bytes);
                    downloadingFileRequest = null;
                    Debug.Log($"{downloadAsset.AssetName} downloaded success. Added to the temp folder");
                }
            }

            onDownloadDone?.Invoke(downloadList);
        }

        public float GetDownloadProgress() {
            if (filesAlreadyDownloaded.Count > 0)
            {
                float totalDownloadSize = GetTotalDownloadFileSize();
                float alreadyDownloadedSize = GetAlreadyDownloadedFileSize();
                float downloadingFileSize = GetDownloadingFileSize();

                return (alreadyDownloadedSize + downloadingFileSize) / totalDownloadSize;
            }

            return 0;
        }

        public float GetTotalDownloadFileSize() {
            if (totalDownloadingFiles.Count > 0)
            {
                return totalDownloadingFiles.Sum(x => x.FileSize);
            }

            return 0;
        }

        public float GetAlreadyDownloadedFileSize() {
            if (filesAlreadyDownloaded.Count > 0)
            {
                return filesAlreadyDownloaded.Sum(x => x.FileSize);
            }

            return 0;
        }

        public float GetDownloadingFileSize() {
            if (downloadingFileRequest != null)
            {
                return downloadingFileRequest.downloadedBytes / 1000.0f;
            }

            return 0;
        }

        public float GetDownloadSpeed() {
            return downloadSpeed;
        }

        public void EnableUpdateDownloadSpeed(float updateInterval = 1) {
            showDownloadSpeedEnabled = true;
        }

        public void DisableUpdateDownloadSpeed() {
            showDownloadSpeedEnabled = false;
        }

        private void Start() {
            StartCoroutine(UpdateDownloadSpeed());
        }

        private IEnumerator UpdateDownloadSpeed()
        {
            while (showDownloadSpeedEnabled && downloadSpeedUpdateTimeInterval > 0)
            {
                float prevDownloadProgress = GetAlreadyDownloadedFileSize() + GetDownloadingFileSize();

                yield return new WaitForSeconds(downloadSpeedUpdateTimeInterval);

                float currentDownloadProgress = GetAlreadyDownloadedFileSize() + GetDownloadingFileSize();

                downloadSpeed = (currentDownloadProgress - prevDownloadProgress) / downloadSpeedUpdateTimeInterval;
            }

            downloadSpeed = -1;
        }
    }
}
