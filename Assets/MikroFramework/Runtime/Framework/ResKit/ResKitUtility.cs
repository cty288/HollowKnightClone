using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using UnityEngine;

namespace MikroFramework.Utilities
{
    public class ResKitUtility {

        /// <summary>
        /// Returns the correct AB path (either local AB path or hot-update AB path) from AB name
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static string GetAssetBundlePath(string assetBundleName) {
            AssetBundleLoadOption loadOption = ResData.Singleton.GetLoadOptionFromABName(assetBundleName);
            if (loadOption == AssetBundleLoadOption.FromLocalFolder) {
                return LocalAssetBundlePath(assetBundleName);
            }else if (loadOption == AssetBundleLoadOption.FromHotUpdateFolder) {
                return HotUpdateAssetBundleFolder + assetBundleName;
            }

            return null;
        }

        /// <summary>
        /// Return the complete path for the local AssetBundle files
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static string LocalAssetBundlePath(string assetBundleName) {
            return HotUpdateConfig.LocalAssetBundleFolder + assetBundleName;
        }



        /// <summary>
        /// Get the folder for Asset Bundles' downloaded path (For hot-update)
        /// </summary>
        public static string HotUpdateAssetBundleFolder {
            get {
                return HotUpdateConfig.HotUpdateAssetBundlesFolder;
            }
        }

        /// <summary>
        /// Get the temp folder that downloaded AB files are saved
        /// </summary>
        public static string TempAssetBundlesPath
        {
            get { return Application.persistentDataPath + "/TempAssetBundles/"; }
        }

        /// <summary>
        /// Generate MD5 of the given file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static String BuildFileMd5(String filePath)
        {
            String filemd5 = null;
            try
            {
                using (var fileStream = File.OpenRead(filePath)) {
                    
                    var md5 = MD5.Create();
                    var fileMD5Bytes = md5.ComputeHash(fileStream);                                  
                    filemd5 = FormatMD5(fileMD5Bytes);
                    
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex);
            }
            return filemd5;
        }

        public static string FormatMD5(Byte[] data)
        {
            return System.BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Get the FileInfo object of a specific AssetBundle file
        /// </summary>
        /// <param name="isLocalPath">Is the bundle located at the local path? If not, it will get from the
        /// hot update folder. Return null if unable to find</param>
        /// <returns></returns>
        public static FileInfo GetABFileInfo(string abName, bool isLocalPath) {
            string basePath = isLocalPath ? LocalAssetBundlePath("") : HotUpdateAssetBundleFolder;
            string fullPath = basePath + abName;
            string relativeDirectory = fullPath.Substring(0, fullPath.LastIndexOf('/'));
            if (!Directory.Exists(relativeDirectory)) {
                return null;
            }

            if (!File.Exists(fullPath)) {
                return null;
            }
            //abname: xxx/yyy or yyy
            string abRealName = fullPath.Substring(fullPath.LastIndexOf('/')+1);
            DirectoryInfo directoryInfo = new DirectoryInfo(relativeDirectory);
            return directoryInfo.GetFiles(abRealName)[0];
        }


     


        /// <summary>
        /// Get all raw file names of a ResVersion file
        /// E.g. (AssetName="test/sprite.ab", raw name="sprite.ab")
        /// </summary>
        /// <param name="resVersion"></param>
        /// <returns></returns>
        public static List<string> GetAllRawAssetNamesFromResVersion(ResVersion resVersion) {
            List<ABMD5Base> assetInfos = resVersion.ABMD5List;
            List<string> results = new List<string>();

            foreach (ABMD5Base assetInfo in assetInfos) {
                string assetName = assetInfo.AssetName; //like test/sprite.ab
                string rawName = assetName.Substring(assetName.LastIndexOf("/") + 1);
                results.Add(rawName);
            }

            return results;
        }

      



        /// <summary>
        /// Get the name of the current platform (Applicable on both Unity Editor and Runtime builds).
        /// </summary>
        public static string CurrentPlatformName {
            get {
#if UNITY_EDITOR
                return GetPlatformName(UnityEditor.EditorUserBuildSettings.activeBuildTarget);
#else
                return GetPlatformName(Application.platform);
#endif
            }
        }


        private static string GetPlatformName(RuntimePlatform runtimePlatform) {
            switch (runtimePlatform) {
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.LinuxPlayer:
                    return "Linux";
                default: return null;
            }
        }

#if UNITY_EDITOR
        private static string GetPlatformName(UnityEditor.BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return "Windows";
                case UnityEditor.BuildTarget.iOS:
                    return "iOS";
                case UnityEditor.BuildTarget.Android:
                    return "Android";
                case UnityEditor.BuildTarget.StandaloneLinux:
                case UnityEditor.BuildTarget.StandaloneLinux64:
                case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
                    return "Linux";
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return "OSX";
                case UnityEditor.BuildTarget.WebGL:
                    return "WebGL";
                default:
                    return null;
            }

        }
#endif
    }
}
