using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MikroFramework.ResKit
{
    public class ResourcesRes : Res
    {

        /// <summary>
        /// Allocate a ResourceRes object from the object pool
        /// </summary>
        /// <returns></returns>
        public static ResourcesRes Allocate(string assetPath, Type resType) {
            ResourcesRes res = SafeObjectPool<ResourcesRes>.Singleton.Allocate();

            res.AssetPath = assetPath.Substring("resources://".Length);
            res.Name = assetPath;
            res.ResType = resType;
            
            return res;
        }


        public override bool LoadSync() {
            State = ResState.Loading;
            Asset = Resources.Load(AssetPath,ResType);
            State = ResState.Loaded;
            return Asset;
        }

        public override void LoadAsync() {
            State = ResState.Loading;
            var request = Resources.LoadAsync(AssetPath,ResType);
            request.completed += operation => {
                Asset = request.asset;
                State = ResState.Loaded;
            };
        }

        protected override void OnReleaseRes() {
            if (Asset is GameObject)
            {
                Asset = null;
                Resources.UnloadUnusedAssets();
            }
            else
            {
                Resources.UnloadAsset(Asset);
            }

            Asset = null;
        }

        public override void RecycleToCache() {
            SafeObjectPool<ResourcesRes>.Singleton.Recycle(this);
        }
    }
}
