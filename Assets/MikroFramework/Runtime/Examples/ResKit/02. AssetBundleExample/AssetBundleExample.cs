using System.Collections;
using System.Collections.Generic;
using System.IO;
using MikroFramework.ResKit;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class AssetBundleExample : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/ResLoader/AssetBundleExample/Build AB", false, 1)]
        private static void MenuItemBuild() {
            if (!Directory.Exists(Application.streamingAssetsPath)) {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            UnityEditor.BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,
                BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }

        [UnityEditor.MenuItem("MikroFramework/Examples/ResLoader/AssetBundleExample/RunAB", false, 2)]
        private static void MenuItemRun() {
            UnityEditor.EditorApplication.isPlaying = true;

            new GameObject("AssetBundleExample").AddComponent<AssetBundleExample>();
        }
#endif
        private AssetBundle bundle;
        private ResLoader resLoader;

        void Start() {
            HotUpdateManager.Singleton.Init(() => {
                HotUpdateManager.Singleton.ValidateHotUpdateState(() => {
                    resLoader = new ResLoader();
                    bundle = resLoader.LoadSync<AssetBundle>("mftest");

                    GameObject gameObject = bundle.LoadAsset<GameObject>("TestABObj");

                    Instantiate(gameObject);
                }, error => {
                    Debug.Log(error.ToString());
                });
            },null);
            

            
        }

        void OnDestroy() {
            bundle = null;
            resLoader.ReleaseAllAssets();
            resLoader = null;
        }
    }
}
