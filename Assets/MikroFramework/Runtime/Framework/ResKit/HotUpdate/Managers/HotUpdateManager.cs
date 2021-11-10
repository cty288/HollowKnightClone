using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using MikroFramework.Managers;
using MikroFramework.Singletons;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace MikroFramework.ResKit
{

    public enum HotUpdateState {
        /// <summary>
        /// Never initialized HotUpdateState
        /// </summary>
        NotInited,
        /// <summary>
        /// Never hot-updated the game (just downloaded it)
        /// </summary>
        NeverUpdated,
        /// <summary>
        /// Hot-updated the game before (but it doesn't mean it's up-to-dated)
        /// Resource version in PersistentDataPath > in StreamingAsset
        /// </summary>
        Updated,
        /// <summary>
        /// Download a new version from asset store (big update), and the hot-update file has not been overridden yet
        /// Resource version in PersistentDataPath < in StreamingAssets
        /// </summary>
        Overridden,
    }

    [MonoSingletonPath("[FrameworkPersistent]/[ResKit]/HotUpdate/HotUpdateManager")]
    public class HotUpdateManager : ManagerBehavior, IHotUpdateManager{
        private HotUpdateState state;
       
        private static HotUpdateManager instance;
        private IHotUpdateDownloader downloader;

        /// <summary>
        /// The downloader of the current hot-update manager
        /// </summary>
        public IHotUpdateDownloader Downloader => downloader;

        public static HotUpdateManager Singleton {
            get {
                if (instance == null) {
                    instance = SingletonProperty<HotUpdateManager>.Singleton;
                }
                return instance;
            }
        }


        /// <summary>
        /// Returns whether the HotUpdateManager exists
        /// </summary>
        /// <returns></returns>
        public static bool Exists() {
            return instance != null;
        }



        void ISingleton.OnSingletonInit() {
            DontDestroyOnLoad(this.gameObject);

            downloader = this.gameObject.AddComponent<HotUpdateDownloader>();
        }

        /// <summary>
        /// Init the HotUpdateManager (recommend to use before using any other APIs), and ValidareHotUpdateState
        /// </summary>
        public void Init(Action onInitFinished, Action<HotUpdateError> onInitFailed) {
            ValidateHotUpdateState(() => {
                ResData.Singleton.Init(null,error => {
                    onInitFailed.Invoke(error);
                });

                onInitFinished?.Invoke();
            }, error => {
                onInitFailed.Invoke(error);
            });
        }

        /// <summary>
        /// The hot-update state of the current game
        /// </summary>
        public HotUpdateState State {
            get {
                if (state == HotUpdateState.NotInited) {
                    Debug.LogError("HotUpdateState never initialized!");
                }

                return state;
            }
        }

        /// <summary>
        /// Check and update the current hot-update state of the game
        /// </summary>
        /// <param name="onCheckFinished"></param>
        public void ValidateHotUpdateState(Action onCheckFinished,Action<HotUpdateError> onError) {
            //Debug.Log("Validating hot update state...");
            var hotUpdateResVersion = HotUpdateConfig.LoadHotUpdateAssetBundlesFolderResVersion();

            if (hotUpdateResVersion==null)
            {
                state = HotUpdateState.NeverUpdated;
                onCheckFinished.Invoke();
            }else {
                //can read/write
                StartCoroutine(downloader.GetLocalAssetResVersion(localResVersion => {
                    if (CommonUtility.AreVersionsEqual(hotUpdateResVersion.Version, localResVersion.Version)) {
                        state = HotUpdateState.Updated;
                    }else if (CommonUtility.CompareVersions(hotUpdateResVersion.Version, localResVersion.Version)) {
                        state = HotUpdateState.Updated;
                    }else if (CommonUtility.CompareVersions(localResVersion.Version, hotUpdateResVersion.Version)) {
                        state = HotUpdateState.Overridden;
                    }

                    
                    onCheckFinished.Invoke();
                }, error => {
                    onError.Invoke(error);
                }));

            }
        }
        


        /// <summary>
        /// Get the ResVersion of the current game on the local computer
        /// </summary>
        /// <param name="getNativeResVersion"></param>
        public void GetNativeResVersion(Action<ResVersion> getNativeResVersion,Action<HotUpdateError> onError) {
            if (state == HotUpdateState.NeverUpdated || state==HotUpdateState.Overridden) {
                StartCoroutine(downloader.GetLocalAssetResVersion(version => {
                    getNativeResVersion.Invoke(version);
                }, error => {
                    onError.Invoke(error);
                }));
                return;
            }
            else {

                ResVersion localResVersionObj = HotUpdateConfig.LoadHotUpdateAssetBundlesFolderResVersion();
                getNativeResVersion.Invoke(localResVersionObj);
            }
            
        }

        /// <summary>
        /// Get if there's a new version on the remote server
        /// </summary>
        /// <param name="onResult">returned result (including the native ResVersion file)</param>
        public void HasNewVersionRes(Action<bool,ResVersion> onResult, Action<HotUpdateError> onError) {

            GetRemoteResVersion(remoteVersion => {
                Debug.Log($"Got remote res version: {remoteVersion.Version}");

                GetNativeResVersion(localResVersion => {
                    Debug.Log($"Got native res version: {localResVersion.Version}");
                    bool result = CommonUtility.CompareVersions(remoteVersion.Version,localResVersion.Version);
                    onResult?.Invoke(result,localResVersion);
                }, error => {
                    onError.Invoke(error);
                });
            }, error => {
                onError.Invoke(error);
            });
        }

        /// <summary>
        /// Download ResVersion file from the remote server
        /// </summary>
        /// <param name="onRemoteResVersionGet"></param>
        public void GetRemoteResVersion(Action<ResVersion> onRemoteResVersionGet, Action<HotUpdateError> onError)
        {
            //Debug.Log($"Getting remote res version...");
            StartCoroutine(downloader.RequestGetRemoteResVersion(resVersion => {
                onRemoteResVersionGet(resVersion);
            }, error => {
                onError.Invoke(error);
            }));
        }

        /// <summary>
        /// Download new resources from the remote server, and replace the local resource
        /// </summary>
        /// <param name="onUpdateDone"></param>
        public void UpdateRes(Action onUpdateDone,Action<HotUpdateError> onUpdateFailed)
        {
           // Debug.Log("Start Update");

            DownloadRes(() => {
                ReplaceLocalRes();
                //Debug.Log("Stop Update");
                onUpdateDone.Invoke();
            },onUpdateFailed);
        }

        /// <summary>
        /// Check the completeness of hot updated files on both local and hot-update folder. Download them
        /// if they are incomplete
        /// return a list of abs that are updated if validate success (return null if nothing to update)
        /// </summary>
        public void ValidateHotUpdateCompleteness(Action<List<ABMD5Base>> onFinished, Action<HotUpdateError> onError) {
            GetRemoteResVersion(remoteResVersion => {

                List<ABMD5Base> needUpdateABs = new List<ABMD5Base>();

                for (int i = 0; i < remoteResVersion.ABMD5List.Count; i++) {
                    ABMD5Base remoteAB = remoteResVersion.ABMD5List[i];
                    bool isComplete = CheckNativeABCompleteness(remoteAB);
                    if (!isComplete) {
                        needUpdateABs.Add(remoteAB);
                    }
                }

                if (needUpdateABs.Count > 0) {
                   
                    StartCoroutine(downloader.DoDownloadRemoteABs(needUpdateABs, (results) => {

                        downloader.DoDownloadResVersion(remoteResVersion);

                        ReplaceLocalRes();
                        //also update ResData
                        foreach (ABMD5Base abmd5Base in results)
                        {

                            //for manifest file (if it's deleted)
                            if (abmd5Base.AssetName != ResKitUtility.CurrentPlatformName)
                            {
                                AssetBundleData abData = ResData.Singleton.GetAssetBundleDataFromABName(abmd5Base.AssetName);
                                if (abData != null) {
                                    abData.LoadOption = AssetBundleLoadOption.FromHotUpdateFolder;
                                }
                                else {
                                    ResData.Singleton.AssetBundleDatas.Add(new AssetBundleData() {
                                        AssetDataList = abmd5Base.assetDatas,
                                        LoadOption = AssetBundleLoadOption.FromHotUpdateFolder,
                                        Name = abmd5Base.AssetName,
                                        MD5 = abmd5Base.MD5
                                    });
                                    
                                }
                                
                            }
                            else
                            {
                                //re-update the manifest file
                                ResData.Singleton.UpdateManifest();
                            }

                        }
                        Debug.Log($"Successfully updated {results.Count} incomplete files to hot-update folder!");
                        onFinished.Invoke(results);
                    }, (error) => {
                        onError.Invoke(error);
                    }));
                }
                else
                {
                    onFinished.Invoke(null);
                }
               
               

            }, error => {
                onError.Invoke(error);
            });
        }

        /// <summary>
        /// Delete redundant AB files on the hot-update folder. Make sure to call it after hot-update
        /// It will not delete those on the local folder.
        /// </summary>
        public void DeleteRedundantFiles(Action onFinished, Action<HotUpdateError> onError) {
            GetRemoteResVersion(resVersion => {
                List<FileInfo> allNativeFiles;
                List<string> resVersionFileNames = ResKitUtility.GetAllRawAssetNamesFromResVersion(resVersion);

                
                allNativeFiles = FileUtility.GetAllDirectoryFiles(
                    ResKitUtility.HotUpdateAssetBundleFolder,
                    true, true);

                if (allNativeFiles == null) {
                    onFinished?.Invoke();
                    return;
                }

                List<FileInfo> filesReadyToDelete = new List<FileInfo>();


                 foreach (FileInfo nativeFile in allNativeFiles) {
                     bool exists = false;
                     //string nativeFileNameFormatted= nativeFile.FullName.Replace('\\', '/');

                     foreach (string resVersionFileName in resVersionFileNames) {
                         if (resVersionFileName == nativeFile.Name ||
                             resVersionFileName + ".manifest" == nativeFile.Name ||
                             resVersionFileName + ".meta" == nativeFile.Name||
                             resVersionFileName+".manifest.meta"==nativeFile.Name) {
                             exists = true;
                             break;
                         }

                         if (nativeFile.Name == "ResVersion.json" || nativeFile.Name ==
                                                                  ResKitUtility.CurrentPlatformName
                                                                  || nativeFile.Name ==
                                                                  ResKitUtility.CurrentPlatformName + ".manifest") {
                             exists = true;
                             break;
                         }
                     }

                     if (!exists) {
                         filesReadyToDelete.Add(nativeFile);
                     }
                 }

                 if (filesReadyToDelete.Count > 0) {
                     foreach (FileInfo fileInfo in filesReadyToDelete)
                     {
                         bool result = FileUtility.DeleteFile(fileInfo);
                         
                         if (!result)
                         {
                             onError.Invoke(HotUpdateError.DeleteRedundentFilesFailed);
                             break;
                         }
                         Debug.Log($"Deleted redundant AB File {fileInfo.Name}");
                    }

                     Debug.Log($"Deleted {filesReadyToDelete.Count} redundant AB files " +
                               $"(including meta and manifest files)!");
                 }else {
                     Debug.Log("No redundant files need to be deleted!");
                 }
                
                 onFinished.Invoke();

            }, error => {
                onError.Invoke(error);
            });
        }


       
        /// <summary>
        /// Enable the hot-update manager to update real-time download speed (this may cost extra memory). 
        /// </summary>
        /// <param name="updateInterval">Update time interval (seconds)</param>
        public void EnableUpdateDownloadSpeed(float updateInterval = 1f) {
            downloader.EnableUpdateDownloadSpeed(updateInterval);
        }

        /// <summary>
        /// Disable the hot-update manager to update real-time download speed
        /// </summary>
        public void DisableUpdateDownloadSpeed() {
            downloader.DisableUpdateDownloadSpeed();
        }

        #region Helpers

        

        private bool CheckNativeABCompleteness(ABMD5Base remoteAB)
        {

            FileInfo hotUpdateABInfo = ResKitUtility.GetABFileInfo(remoteAB.AssetName, false);

            if (hotUpdateABInfo != null)
            {
                if (IsMD5Equal(hotUpdateABInfo, remoteAB.MD5))
                {
                    Debug.Log($"hot-updated {hotUpdateABInfo.Name} is complete!");
                    return true;
                }

                Debug.Log($"{hotUpdateABInfo.Name} is incomplete!");
                return false;
            }
            else
            {
                FileInfo localUpdateABInfo = ResKitUtility.GetABFileInfo(remoteAB.AssetName, true);
                if (localUpdateABInfo != null)
                {
                    if (IsMD5Equal(localUpdateABInfo, remoteAB.MD5))
                    {
                        Debug.Log($"local {localUpdateABInfo.Name} is complete!");
                        return true;
                    }
                    else
                    {
                        Debug.Log($"{localUpdateABInfo.Name} is incomplete!");
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"{remoteAB.AssetName} does not exist!");
                    return false; //unable to find ab on native
                }

            }
        }

        private bool IsMD5Equal(FileInfo comparedFileInfo, string targetMD5)
        {
            string md5 = ResKitUtility.BuildFileMd5(comparedFileInfo.FullName);
            return md5 == targetMD5;
        }
        /// <summary>
        /// Download and save the ResVersion file, and other ABs that needs to be updated from the remote server
        /// </summary>
        /// <param name="onResDownloaded"></param>
        private void DownloadRes(Action onResDownloaded, Action<HotUpdateError> ondownloadFailed)
        {
            StartCoroutine(downloader.RequestGetRemoteResVersion(remoteResVersion => {
                downloader.DoDownloadResVersion(remoteResVersion);
                List<AssetBundleData> nativeABDatas = ResData.Singleton.AssetBundleDatas;

                List<ABMD5Base> needUpdateFileInfos = CollectNeedUpdateABs(nativeABDatas, remoteResVersion);

                StartCoroutine(downloader.DoDownloadRemoteABs(needUpdateFileInfos, (results) => {
                    UpdateABDatas(nativeABDatas, results);
                    onResDownloaded.Invoke();
                }, error => {
                    ondownloadFailed.Invoke(error);
                }));
            }, error => {
                ondownloadFailed.Invoke(error);
            }));
        }

        private List<ABMD5Base> CollectNeedUpdateABs(List<AssetBundleData> nativeABDatas, ResVersion remoteResVersion)
        {

            List<ABMD5Base> needUpdateFileInfos = new List<ABMD5Base>();

            for (int i = 0; i < remoteResVersion.ABMD5List.Count; i++)
            {
                ABMD5Base remoteAbmd5 = remoteResVersion.ABMD5List[i];
                bool existsInNative = false;

                for (int j = 0; j < nativeABDatas.Count; j++)
                {
                    AssetBundleData nativeABData = nativeABDatas[j];

                    if (remoteAbmd5.AssetName == nativeABData.Name)
                    {
                        

                        existsInNative = true;
                        //Debug.Log($"RemoteABMD5 {remoteAbmd5.AssetName} has MD5 {remoteAbmd5.MD5};" +
                        //  $"while the one on the native has MD5 {nativeABData.MD5}");
                        //always update manifest file
                        if (remoteAbmd5.MD5 != nativeABData.MD5 || remoteAbmd5.AssetName == ResKitUtility.CurrentPlatformName)
                        {
                            needUpdateFileInfos.Add(remoteAbmd5);
                            //Debug.Log($"{remoteAbmd5.AssetName} needs update. It will be updated later");
                        }
                    }
                }

                if (!existsInNative)
                {
                    needUpdateFileInfos.Add(remoteAbmd5);
                    Debug.Log($"Find a new AB {remoteAbmd5.AssetName} on the server. It wil be downloaded later");
                }
            }

            return needUpdateFileInfos;
        }

        private void UpdateABDatas(List<AssetBundleData> nativeABDatas, List<ABMD5Base> needUpdateABs)
        {
            for (int i = 0; i < needUpdateABs.Count; i++)
            {
                ABMD5Base needUpdateAB = needUpdateABs[i];

                bool exists = false;
                for (int j = 0; j < nativeABDatas.Count; j++)
                {
                    AssetBundleData nativeABData = nativeABDatas[j];

                    if (needUpdateAB.AssetName == nativeABData.Name)
                    {
                        exists = true;
                        nativeABData.LoadOption = AssetBundleLoadOption.FromHotUpdateFolder;
                        nativeABData.MD5 = needUpdateAB.MD5;
                        // Debug.Log($"Already updated existing AB data {needUpdateAB.AssetName}");
                    }
                }

                if (!exists)
                {
                    AssetBundleData newABData = new AssetBundleData()
                    {
                        LoadOption = AssetBundleLoadOption.FromHotUpdateFolder,
                        MD5 = needUpdateAB.MD5,
                        Name = needUpdateAB.AssetName
                    };
                    ResData.Singleton.AssetBundleDatas.Add(newABData);
                    //Debug.Log($"Added a new AB data {needUpdateAB.AssetName}");
                }
            }
        }

        private void ReplaceLocalRes()
        {
            //Debug.Log("2. Replacing Assets from Temp assets");
            string tempAssetBundleFolder = ResKitUtility.TempAssetBundlesPath;
            string hotUpdateAssetBundleFolder = ResKitUtility.HotUpdateAssetBundleFolder;


            DirectoryInfo directoryInfo = new DirectoryInfo(tempAssetBundleFolder);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string fullPath = files[i].FullName;
                string relativePath = fullPath.Substring(ResKitUtility.TempAssetBundlesPath.Length - 1);
                relativePath = relativePath.Replace('\\', '/');

                string fullDirectory;

                if (relativePath.Length == 1)
                {
                    fullDirectory = hotUpdateAssetBundleFolder;
                }
                else
                {
                    string relativeDirectory = relativePath.Substring(0, relativePath.LastIndexOf('/'));
                    fullDirectory = hotUpdateAssetBundleFolder + relativeDirectory + "/";
                }

                if (!Directory.Exists(fullDirectory))
                {
                    Directory.CreateDirectory(fullDirectory);
                }

                File.Copy(files[i].FullName, fullDirectory +
                                             files[i].Name, true);
            }

            if (Directory.Exists(tempAssetBundleFolder))
            {

                Directory.Delete(tempAssetBundleFolder, true);
            }

        }
        #endregion

        
    }
}

