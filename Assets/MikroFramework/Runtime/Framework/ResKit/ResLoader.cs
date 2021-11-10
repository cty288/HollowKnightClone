using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MikroFramework.ResKit
{
    /// <summary>
    /// Use ResLoader.Create() instead to avoid bugs
    /// </summary>
    public class ResLoader:IUtility, IPoolable {


        private ResTable recordedRes = new ResTable();


        public static ResLoader Allocate() {
            ResLoader resLoader = SafeObjectPool<ResLoader>.Singleton.Allocate();
            return resLoader;
        }

        /// <summary>
        /// Use ResLoader.Create() instead to avoid bugs
        /// </summary>
        public ResLoader() {
            if (!ResData.Exists)
            {
                //resdata does not exist, initialize a new resdata
                ResData.Singleton.Init(null, null);
            }
        }

        /// <summary>
        /// Init and create a resloader. This may take some time.
        /// </summary>
        /// <param name="onInitFinished"></param>
        public static void Create(Action<ResLoader> onInitFinished) {
            if (!ResData.Exists) {
                //resdata does not exist, initialize a new resdata
                ResData.Singleton.Init(() => {
                    onInitFinished?.Invoke(new ResLoader());
                },(error) => {
                    Debug.LogError(error);
                });
            }
            else {
                onInitFinished?.Invoke(new ResLoader());
            }
        }

        #region Helpers
        private Res GetResFromRecord(ResSearchKeys resSearchKeys) {
            Res retResult = recordedRes.GetResWithSearchKeys(resSearchKeys);

            return retResult;
        }

        private Res GetResFromResManager(ResSearchKeys resSearchKeys) {
            return ResManager.GetRes(resSearchKeys);
        }

        private void AddResToRecord(Res resFromResManager)
        {
            recordedRes.Add(resFromResManager);
            resFromResManager.Retain();
        }

        /// <summary>
        /// Customize your Resource file path header here
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private Res CreateRes(ResSearchKeys resSearchKeys) {
            Res res = null;

            res = ResFactory.CreateRes(resSearchKeys);

            AddResToRecord(res);
            ResManager.AddRes(res);
            return res;
        }

        private Res GetRes(ResSearchKeys resSearchKeys) {
            //local resource pool
            Res res = GetResFromRecord(resSearchKeys);
            if (res != null)
            {
                return res;
            }

            //global resource pool
            res = GetResFromResManager(resSearchKeys);
            if (res != null)
            {
                AddResToRecord(res);
                return res;
            }

            
            return null;
        }

        private T DoLoadSync<T>(ResSearchKeys resSearchKeys) where T : Object {
            var res = GetRes(resSearchKeys);
            Debug.Log(resSearchKeys.ResType);
            if (res != null)
            {
                if (res.State == ResState.Loading) {
                    throw new Exception($"Do not load resource {res.Name} synchronously while it's loading" +
                                        $" asynchronously!");
                }
                else if (res.State == ResState.Loaded)
                {
                    resSearchKeys.RecycleToCache();
                    return res.Asset as T;
                }
            }

            res = CreateRes(resSearchKeys);
            res.LoadSync();
            resSearchKeys.RecycleToCache();
            return res.Asset as T;
        }

        private void DoLoadAsync<T>(ResSearchKeys resSearchKeys, Action<T> onLoaded) where T:Object{
            //local resource pool
            var res = GetRes(resSearchKeys);

            //event triggered after the resource is loaded: triggered onloaded; unregister resource's loaded event
            Action<Res> onResLoaded = null;
            onResLoaded = (loadedRes) => {
                onLoaded?.Invoke(loadedRes.Asset as T);
                res.UnRegisterOnLoadedEvent(onResLoaded);
            };


            if (res != null)
            {
                switch (res.State)
                {
                    //resource is still loading: listen to resource's load success event.
                    case ResState.Loading:
                        res.RegisterOnLoadedEvent(onResLoaded);
                        break;
                    case ResState.Loaded:
                        onLoaded(res.Asset as T);
                        break;
                }
                resSearchKeys.RecycleToCache();
                return;
            }

            //no such a resource loaded in the game: Create a new Res Object, add listener to
            //Res Object's load success event, and let Res to load the real resource asyncly 
            
            res = CreateRes(resSearchKeys);


            res.RegisterOnLoadedEvent(onResLoaded);

            res.LoadAsync();

            resSearchKeys.RecycleToCache();
        }

        #endregion

        /// <summary>
        /// Load an asset from AssetBundle, as well as the AssetBundle itself. Return the loaded asset.
        /// If the asset is already loaded somewhere else, it will not load again but only return the asset
        /// </summary>
        /// <returns></returns>
        public T LoadSync<T>(string assetBundleName, string assetName) where T : Object {
            return DoLoadSync<T>(ResSearchKeys.Allocate(assetName,typeof(T),assetBundleName));
        }


        /// <summary>
        /// Load an asset (internal supported: Resources and AssetBundle) folder to the memory. Return the loaded asset.
        /// If the asset is already loaded somewhere else, it will not load again but only return the asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPathOrName">for Resources assets: assetPathOrName must start with header "resources://"
        /// for AssetBundle assets: assetPath do not need a header
        /// for other customized assets: define them in ResFactory</param>
        /// <returns></returns>
        public T LoadSync<T>(string assetPathOrName) where T : Object {
            return DoLoadSync<T>(ResSearchKeys.Allocate(assetPathOrName,typeof(T)));
        }


        /// <summary>
        /// Load an asset from AssetBundle asynchronously, as well as the AssetBundle itself. Return the loaded asset.
        /// If the asset is already loaded somewhere else, it will not load again but only return the asset
        /// </summary>
        /// <param name="onLoaded">Event triggered when loaded success</param>
        /// <returns></returns>
        public void LoadAsync<T>(string assetBundleName, string assetName, Action<T> onLoaded) where T : Object {
            DoLoadAsync<T>(ResSearchKeys.Allocate(assetName,typeof(T),assetBundleName), onLoaded);
        }

        /// <summary>
        /// Load an asset (internal supported: Resources and AssetBundle) to the memory asynchronously. Return the loaded asset.
        /// If the asset is already loaded somewhere else, it will not load again but only return the asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resSearchKeys">for Resources assets: assetAddress must start with header "resources://resourcepath"
        /// for AssetBundle assets: assetPath do not need a header
        /// for other customized assets: define them in ResFactory</param>
        /// <param name="onLoaded">Event triggered when loaded success</param>
        /// <returns></returns>
        public void LoadAsync<T>(string assetPathOrName,Action<T> onLoaded) where T : Object {
            DoLoadAsync<T>(ResSearchKeys.Allocate(assetPathOrName,typeof(T)),onLoaded);
        }


        /// <summary>
        /// Release all assets of this Resource Loader. This will not unload the same assets on other Resource Loaders
        /// </summary>
        public void ReleaseAllAssets() {

            foreach (Res res in recordedRes) {
                res.Release();
            }

            recordedRes.Clear();
            (this as IPoolable).RecycleToCache();
        }

        void IPoolable.OnRecycled() {
            
        }

        public bool IsRecycled { get; set; }
        
        void  IPoolable.RecycleToCache() {
            SafeObjectPool<ResLoader>.Singleton.Recycle(this);
        }

        
    }
}
