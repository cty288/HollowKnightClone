using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MikroFramework.ResKit
{
    /// <summary>
    /// Single resource that loaded from a AssetBundle. 
    /// </summary>
    public class AssetRes : Res {
        private string ownerBundleName;
        private ResLoader resLoader = new ResLoader();

        private string nameInAB;

        /// <summary>
        /// Allocate a AssetRes Object from the object pool
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="ownerBundleName"></param>
        /// <returns></returns>
        public static AssetRes Allocate(string assetName, string ownerBundleName, Type resType) {
            AssetRes res = SafeObjectPool<AssetRes>.Singleton.Allocate();

            res.nameInAB = assetName;

            if (String.IsNullOrEmpty(ownerBundleName)) {
                ResSearchKeys resSearchKeys = ResSearchKeys.Allocate(assetName,resType);
                ownerBundleName = ResData.Singleton.GetAssetData(resSearchKeys)
                    .OwnerBundleName;

                resSearchKeys.RecycleToCache();
            }
            
            res.Name = ownerBundleName + "/" + assetName;
            res.ResType = resType;
            res.ownerBundleName = ownerBundleName;
            res.State = ResState.Waiting;
            return res;
        }

      

        public override void LoadAsync() {
            State = ResState.Loading;
            
            resLoader.LoadAsync<AssetBundle>(ownerBundleName, bundle => {
                if (ResManager.IsSimulationModeLogic) {
#if UNITY_EDITOR
                    string[] assetPaths = 
                        UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(ownerBundleName, nameInAB);
                    Asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPaths[0]);
                    State = ResState.Loaded;
#endif
                }
                else {
                    AssetBundleRequest request = bundle.LoadAssetAsync(nameInAB);
                    request.completed += operation => {
                        Asset = request.asset;
                        State = ResState.Loaded;
                    };
                }
                 
            });

        }

        public override bool MatchResSearchKeysWithoutName(ResSearchKeys resSearchKeys) {
            return resSearchKeys.OwnerBundleName == ownerBundleName && resSearchKeys.ResType == ResType;
        }

        public override bool LoadSync()
        {
            State = ResState.Loading;
            AssetBundle ownerBundle = resLoader.LoadSync<AssetBundle>(this.ownerBundleName);

            if (ResManager.IsSimulationModeLogic) {
#if UNITY_EDITOR
                string[] assetPaths =
                    UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(ownerBundleName, nameInAB);

                Asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPaths[0]);
#endif
            }
            else {
                Asset = ownerBundle.LoadAsset(nameInAB);
            }


            
            State = ResState.Loaded;
            return Asset;
        }

        protected override void OnReleaseRes()
        {
            if (Asset is GameObject) {

            }
            else {
                Resources.UnloadAsset(Asset);
            }

            Asset = null;
            resLoader.ReleaseAllAssets();
            resLoader = null;
        }

        public override void RecycleToCache() {
            SafeObjectPool<AssetRes>.Singleton.Recycle(this);
        }
    }
}
