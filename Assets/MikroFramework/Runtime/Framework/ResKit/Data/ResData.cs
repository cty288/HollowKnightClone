
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikroFramework.Singletons;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MikroFramework.ResKit {


    /// <summary>
    /// Class that manage Project AB Manifest and saves all AB's load path (from local or hot-update manager)
    /// </summary>
    [MonoSingletonPath("[FrameworkPersistent]/[ResKit]/ResData")]
    public class ResData : MonoPersistentMikroSingleton<ResData> {
        private static AssetBundleManifest manifest;
        private static AssetBundle manifestBundle;

        private Action<HotUpdateError> onError;

       

        /// <summary>
        /// Initialize the ResData singleton for hot-update
        /// </summary>
        public void Init(Action onFinished, Action<HotUpdateError> onInitError) {
            onError += onInitError;
            Load(onFinished);
        }

        protected override void OnBeforeDestroy() {
            base.OnBeforeDestroy();
            if (manifestBundle != null)
            {
                manifestBundle.Unload(true);
            }
        }

        /// <summary>
        /// Contains all native AB package datas, including names, MD5...
        /// </summary>
        public List<AssetBundleData> AssetBundleDatas = new List<AssetBundleData>();

        private AssetDataTable assetDataTable = new AssetDataTable();

        /// <summary>
        /// Get an AssetData only by its name and type, does not need ownerBundleName
        /// </summary>
        /// <param name="resSearchKeys"></param>
        /// <returns></returns>
        public AssetData GetAssetData(ResSearchKeys resSearchKeys) {
            AssetData ret= assetDataTable.GetAssetDataByResSearchKeys(resSearchKeys);
           
            return ret;
        }

        public string[] GetDirectDependencies(string bundleName) {
            if (ResManager.IsSimulationModeLogic) {
                return AssetBundleDatas.Find(abData => abData.Name == bundleName)
                    .DependencyBundleNames;
            }

            return manifest.GetDirectDependencies(bundleName);
        }

        public AssetBundleData GetAssetBundleDataFromABName(string abName) {
            foreach (AssetBundleData assetBundleData in AssetBundleDatas) {
                if (assetBundleData.Name == abName) {
                    return assetBundleData;
                }
            }

            return null;
        }

        public AssetBundleLoadOption GetLoadOptionFromABName(string abName) {
            foreach (AssetBundleData assetBundleData in AssetBundleDatas) {
                if (assetBundleData.Name == abName) {
                    return assetBundleData.LoadOption;
                }
            }

            return AssetBundleLoadOption.UnKnown;
        }

        /// <summary>
        /// Re-update the manifest variable of ResData (called in HotUpdateManager, considered that some
        /// players may delete it)
        /// </summary>
        public void UpdateManifest() {
            string filePath = ResKitUtility.GetAssetBundlePath(ResKitUtility.CurrentPlatformName);

            if (File.Exists(filePath)) {
                if (manifestBundle != null) {
                    manifestBundle.Unload(true);
                }
                manifestBundle =
                    UnityEngine.AssetBundle.LoadFromFile(filePath);
                manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                Debug.Log($"Updated Manifest File: {filePath}");
            }
        }

        private void Load(Action onFinished) {
            AssetBundleDatas.Clear();
            assetDataTable.Clear();
            if (ResManager.IsSimulationModeLogic) {
#if UNITY_EDITOR
                string[] abNames = UnityEditor.AssetDatabase.GetAllAssetBundleNames();

                foreach (string abName in abNames) {
                    string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(abName);

                    AssetBundleData assetBundleData = new AssetBundleData() {
                        Name = abName,
                        DependencyBundleNames = UnityEditor.AssetDatabase.GetAssetBundleDependencies(abName, false),
                        LoadOption = AssetBundleLoadOption.Simulation
                    };

                    foreach (string assetPath in assetPaths) {
                        AssetData data = new AssetData();

                        data.Name = assetPath.Split('/').Last().Split('.').First();
                        data.OwnerBundleName = abName;
                        data.AssetType = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                        assetBundleData.AssetDataList.Add(data);

                        assetDataTable.Add(data);
                    }

                    AssetBundleDatas.Add(assetBundleData);
                }

                AssetBundleDatas.ForEach(abData => {
                    Debug.Log($"----------AssetBundle: {abData.Name}------------");

                    abData.AssetDataList.ForEach(asset => { Debug.Log($"[{abData.Name}]: Loaded {asset.Name}"); });

                    foreach (string abDataDependencyBundleName in abData.DependencyBundleNames) {
                        Debug.Log($"[{abData.Name}]: Dependency: {abDataDependencyBundleName}");
                    }

                });

                onFinished?.Invoke();

#endif
            }
            else {
                Debug.Log("ResData start Loading");

                bool hasHotUpdate = HotUpdateManager.Exists();
                Debug.Log($"HotUpdate Manager exists: {hasHotUpdate}");

                if (hasHotUpdate) {
                    HotUpdateState hotUpdateState = HotUpdateManager.Singleton.State;
                    if (hotUpdateState == HotUpdateState.NeverUpdated || hotUpdateState == HotUpdateState.Overridden)
                    {
                        StartCoroutine(HotUpdateManager.Singleton.Downloader.GetLocalAssetResVersion(resVersion => {
                            foreach (ABMD5Base abmd5Base in resVersion.ABMD5List)
                            {
                                AssetBundleData data = new AssetBundleData()
                                {
                                    Name = abmd5Base.AssetName,
                                    MD5 = abmd5Base.MD5,
                                    LoadOption = AssetBundleLoadOption.FromLocalFolder,
                                    AssetDataList = abmd5Base.assetDatas 
                                };
                                AssetBundleDatas.Add(data);
                                assetDataTable.Add(abmd5Base.assetDatas);
                            }
                            UpdateManifest();
                            onFinished?.Invoke();
                        }, error => {
                            onError.Invoke(error);
                        }));
                    }
                    else if (hotUpdateState == HotUpdateState.Updated)
                    {
                        

                        string hotUpdateFolder = ResKitUtility.HotUpdateAssetBundleFolder;
                        DirectoryInfo directoryInfo = new DirectoryInfo(hotUpdateFolder);
                        FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);





                        ResVersion hotUpdateFolderResVersion = HotUpdateConfig.LoadHotUpdateAssetBundlesFolderResVersion();
                        List<string> abNamesInHotUpdateFolder = new List<string>();

                        for (int i = 0; i < hotUpdateFolderResVersion.ABMD5List.Count; i++)
                        {
                            ABMD5Base abmd5Base = hotUpdateFolderResVersion.ABMD5List[i];

                            for (int j = 0; j < files.Length; j++)
                            {
                                //get file's "real name" (e.g.: AssetName: xxx/yyy, realName: yyy)
                                string realName = abmd5Base.AssetName.Substring(abmd5Base.AssetName.LastIndexOf('/') + 1);

                                if (files[j].Name == realName)
                                {
                                    AssetBundleData assetBundleData = new AssetBundleData()
                                    {
                                        Name = abmd5Base.AssetName,
                                        MD5 = abmd5Base.MD5,
                                        LoadOption = AssetBundleLoadOption.FromHotUpdateFolder,
                                        AssetDataList = abmd5Base.assetDatas
                                    };
                                    //Debug.Log(abmd5Base.assetDatas);
                                    abNamesInHotUpdateFolder.Add(abmd5Base.AssetName);
                                    AssetBundleDatas.Add(assetBundleData);
                                    assetDataTable.Add(abmd5Base.assetDatas);

                                   
                                    break;
                                }
                            }
                        }

                        for (int i = 0; i < hotUpdateFolderResVersion.ABMD5List.Count; i++)
                        {
                            bool exists = false;
                            ABMD5Base abmd5Base = hotUpdateFolderResVersion.ABMD5List[i];

                            for (int j = 0; j < abNamesInHotUpdateFolder.Count; j++)
                            {
                                if (abmd5Base.AssetName == abNamesInHotUpdateFolder[j])
                                {
                                    exists = true;
                                }
                            }

                            if (!exists)
                            {
                                AssetBundleData assetBundleData = new AssetBundleData()
                                {
                                    Name = abmd5Base.AssetName,
                                    MD5 = abmd5Base.MD5,
                                    LoadOption = AssetBundleLoadOption.FromLocalFolder,
                                    AssetDataList = abmd5Base.assetDatas
                                };
                                AssetBundleDatas.Add(assetBundleData);
                                assetDataTable.Add(abmd5Base.assetDatas);
                            }
                        }
                        foreach (AssetData data in assetDataTable)
                        {
                            Debug.Log(data.Name + "     " + data.OwnerBundleName+"    "+data.AssetType.Name);
                        }
                        UpdateManifest();
                        onFinished?.Invoke();
                    }
                }else { //no hotupdate manager, local from local
                    Debug.Log("No Hotupdate Manager");
                    if (manifest == null) {
                        AssetBundleDatas.Add(new AssetBundleData()
                        {
                            Name = ResKitUtility.CurrentPlatformName,
                            LoadOption = AssetBundleLoadOption.FromLocalFolder
                        });

                        UpdateManifest();

                        if (manifest != null) {
                            HotUpdateDownloader downloader = new HotUpdateDownloader();
                            StartCoroutine(downloader.GetLocalAssetResVersion(resVersion => {
                                foreach (ABMD5Base abmd5Base in resVersion.ABMD5List)
                                {
                                    AssetBundleData data = new AssetBundleData()
                                    {
                                        Name = abmd5Base.AssetName,
                                        MD5 = abmd5Base.MD5,
                                        LoadOption = AssetBundleLoadOption.FromLocalFolder,
                                        AssetDataList = abmd5Base.assetDatas
                                    };
                                    AssetBundleDatas.Add(data);
                                    assetDataTable.Add(abmd5Base.assetDatas);
                                }
                                onFinished?.Invoke();
                            }, error => {
                                onError.Invoke(error);
                            }));

                        }
                    }
                    
                    
                }
                

            }
        }

    }
}
