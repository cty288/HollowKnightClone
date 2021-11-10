#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikroFramework.Serializer;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;
using EditorUtility = MikroFramework.Utilities.EditorUtility;

namespace MikroFramework.ResKit
{

    public class AssetBundleExporter : MonoBehaviour
    {


        [MenuItem("MikroFramework/Framework/ResKit/Build AssetBundles to Local Path", false,1)]
        private static void NormalBuild() {
            BuildAB();
        }

        [MenuItem("MikroFramework/Framework/ResKit/Build AssetBundles to Asset Data Path", false,2)]
        private static void ABBuild()
        {
            BuildAB(true);
        }


        public static void BuildAB(bool isHotUpdate=false) {
            string outputPath = "";
            if (!isHotUpdate) {
                 outputPath= HotUpdateConfig.LocalAssetBundleFolder;
            }
            else {
                outputPath = HotUpdateConfig.AssetBundleAssetDataBuildPath;
            }
            

            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();

            EditorUtility.OpenInFolder(outputPath);


            WriteVersionConfig(outputPath);
            
        }

        private static void WriteVersionConfig(string outPutPath) {
            UnityEditor.EditorApplication.isPlaying = true;
            bool latestSimulationMode = ResManager.SimulationMode;
            ResManager.SimulationMode = true;
            ResData.Singleton.Init(null,(e)=>{});

            string versionConfigFilePath = outPutPath + "ResVersion.json";

            if (File.Exists(versionConfigFilePath))
            {
                File.Delete(versionConfigFilePath);
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(outPutPath);

            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            
            ResVersion resVersion = new ResVersion();
            resVersion.ABMD5List = new List<ABMD5Base>();

            List<AssetBundleData> assetBundleDatas = ResData.Singleton.AssetBundleDatas;
            assetBundleDatas.Add(new AssetBundleData() {
                Name = ResKitUtility.CurrentPlatformName
            });
            

            for (int i = 0; i < files.Length; i++) {
                if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".manifest")) {
                    string fileCompletePath = files[i].FullName;
                    fileCompletePath = fileCompletePath.Replace('\\', '/');
                    string fileAssetPath= fileCompletePath.Substring(outPutPath.Length);

                    foreach (AssetBundleData assetBundleData in assetBundleDatas) {
                        if (assetBundleData.Name == fileAssetPath) {

                            ABMD5Base abmd5Base = new ABMD5Base()
                            {
                                AssetName = fileAssetPath,
                                MD5 = ResKitUtility.BuildFileMd5(files[i].FullName),
                                FileSize = files[i].Length / 1024.0f,
                                assetDatas = assetBundleData.AssetDataList
                            };

                            resVersion.ABMD5List.Add(abmd5Base);
                            break;
                        }
                    }
                    
                    
                }
            }

            resVersion.Version = Application.version;
            string resVersionJson = AdvancedJsonSerializer.Singleton.Serialize(resVersion);

            
            File.WriteAllText(versionConfigFilePath, resVersionJson);
            UnityEditor.EditorApplication.isPlaying = false;
            ResManager.SimulationMode = latestSimulationMode;
            
        }
    }
}
#endif