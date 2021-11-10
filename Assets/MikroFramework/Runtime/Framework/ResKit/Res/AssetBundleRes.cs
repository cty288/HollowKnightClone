using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using MikroFramework.Utilities;
using UnityEngine;

namespace MikroFramework.ResKit
{
    public class AssetBundleRes : Res {
       
        

        public AssetBundle AssetBundle {
            get {
                return Asset as AssetBundle;
            }
            set {
                Asset = value;
            }
        }

        private ResLoader resLoader = new ResLoader();

        /// <summary>
        /// Allocate a new AssetBundleRes from the Object pool
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static AssetBundleRes Allocate(string assetName) {
            AssetBundleRes res = SafeObjectPool<AssetBundleRes>.Singleton.Allocate();

            res.AssetPath= ResKitUtility.GetAssetBundlePath(assetName);
            res.ResType = typeof(AssetBundle);
            res.Name = assetName;
            res.State = ResState.Waiting;
            return res;
        }

       

        private void LoadDependencyBundlesAsync(Action onAllLoaded) {
            int loadedCount = 0;
            string[] dependencyBundleNames = ResData.Singleton.GetDirectDependencies(Name);

            if (dependencyBundleNames.Length == 0) {
                onAllLoaded?.Invoke();
            }

            foreach (var bundleName in dependencyBundleNames)
            {
                resLoader.LoadAsync<AssetBundle>(bundleName,
                    (ab) => {
                        loadedCount++;

                        if (loadedCount == dependencyBundleNames.Length)
                        {
                            onAllLoaded?.Invoke();
                        }
                    });
                Debug.Log($"Loaded Dependency: {bundleName}");
            }
        }



        public override void LoadAsync() {
            State = ResState.Loading;

            if (ResManager.IsSimulationModeLogic) {
                State = ResState.Loaded;
            }
            else {
                LoadDependencyBundlesAsync(() => {
                    var request = AssetBundle.LoadFromFileAsync(AssetPath);

                    request.completed += operation => {
                        Asset = request.assetBundle;
                        State = ResState.Loaded;
                    };
                });
            }
            
        }

        public override bool LoadSync() {
            State = ResState.Loading;
            string[] dependencyBundleNames = ResData.Singleton.GetDirectDependencies(Name);

            foreach (var bundleName in dependencyBundleNames) {
                resLoader.LoadSync<AssetBundle>(bundleName);
                Debug.Log($"Loaded Dependency: {bundleName}");
            }

            if (!ResManager.IsSimulationModeLogic) {
                AssetBundle = AssetBundle.LoadFromFile(AssetPath);
            }

            State = ResState.Loaded;
            return AssetBundle;
        }

        protected override void OnReleaseRes()
        {
            if (AssetBundle != null) {
                AssetBundle.Unload(true);

                resLoader.ReleaseAllAssets();
                resLoader = null;
            }


            
            AssetBundle = null;
        }

        public override void RecycleToCache() {
            SafeObjectPool<AssetBundleRes>.Singleton.Recycle(this);
        }


    }
}
