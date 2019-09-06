﻿using Dot.Core.Util;
using System;
using UnityEngine;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetManager : Singleton<AssetManager>
    {
        private AAssetLoader assetLoader = null;
        private bool isInit = false;
        public void InitLoader(AssetLoaderMode loaderMode, AssetPathMode pathMode, Action<bool> initCallback,params SystemObject[] sysObjs)
        {
            if(loaderMode == AssetLoaderMode.Resources)
            {
                assetLoader = new ResourceLoader();
            }else if(loaderMode == AssetLoaderMode.AssetBundle)
            {
                assetLoader = new AssetBundleLoader();
            }
            else if (loaderMode == AssetLoaderMode.AssetDatabase)
            {
#if UNITY_EDITOR
                assetLoader = new AssetDatabaseLoader();
#else
                Debug.LogError("AssetManager::InitLoader->AssetLoaderMode(AssetDatabase) can be used in Editor");
#endif
            }
            assetLoader?.Initialize(pathMode,(isSuccess) =>
            {
                isInit = isSuccess;
                initCallback?.Invoke(isSuccess);
            }, sysObjs);
        }

        public AssetLoaderHandle LoadAssetAsync(
            string assetPath,
            OnAssetLoadComplete complete, 
            AssetLoaderPriority priority = AssetLoaderPriority.Default,  
            OnAssetLoadProgress progress = null,
            SystemObject userData = null)
        {
            if(isInit)
            {
                return assetLoader.LoadOrInstanceBatchAssetAsync(new string[] { assetPath }, false, priority, complete, progress, null, null, userData);
            }else
            {
                Debug.LogError("AssetManager::LoadAssetAsync->init is failed");
                return null;
            }
        }

        public AssetLoaderHandle LoadBatchAssetAsync(
            string[] assetPaths,
            OnAssetLoadComplete complete,
            OnBatchAssetLoadComplete batchComplete,
            AssetLoaderPriority priority = AssetLoaderPriority.Default,
            OnAssetLoadProgress progress = null,
            OnBatchAssetsLoadProgress batchProgress = null,
            SystemObject userData = null)
        {
            if (isInit)
            {
                return assetLoader.LoadOrInstanceBatchAssetAsync(assetPaths, false, priority, complete, progress, batchComplete, batchProgress, userData);
            }
            else
            {
                Debug.LogError("AssetManager::LoadAssetAsync->init is failed");
                return null;
            }
        }

        public AssetLoaderHandle InstanceAssetAsync(
            string assetPath,
            OnAssetLoadComplete complete,
            AssetLoaderPriority priority = AssetLoaderPriority.Default,
            OnAssetLoadProgress progress = null,
            SystemObject userData = null)
        {
            if (isInit)
            {
                return assetLoader.LoadOrInstanceBatchAssetAsync(new string[] { assetPath }, true, priority, complete, progress, null, null, userData);
            }
            else
            {
                Debug.LogError("AssetManager::LoadAssetAsync->init is failed");
                return null;
            }
        }

        public AssetLoaderHandle InstanceBatchAssetAsync(
            string[] assetPaths,
            OnAssetLoadComplete complete,
            OnBatchAssetLoadComplete batchComplete,
            AssetLoaderPriority priority = AssetLoaderPriority.Default,
            OnAssetLoadProgress progress = null,
            OnBatchAssetsLoadProgress batchProgress = null,
            SystemObject userData = null)
        {
            if (isInit)
            {
                return assetLoader.LoadOrInstanceBatchAssetAsync(assetPaths, true, priority, complete, progress, batchComplete, batchProgress, userData);
            }
            else
            {
                Debug.LogError("AssetManager::LoadAssetAsync->init is failed");
                return null;
            }
        }

        public UnityObject InstantiateAsset(string assetPath,UnityObject asset)
        {
            if (isInit)
            {
                return assetLoader?.InstantiateAsset(assetPath, asset);
            }
            else
            {
                Debug.LogError("AssetManager::InstantiateAsset->init is failed");
                return null;
            }
        }

        public void UnloadUnusedAsset(Action callback = null)
        {
            if (isInit)
            {
                assetLoader?.UnloadUnusedAssets(callback);
            }
            else
            {
                Debug.LogError("AssetManager::InstantiateAsset->init is failed");
            }
        }

        public void UnloadAssetLoader(AssetLoaderHandle handle)
        {
            if (isInit)
            {
                assetLoader?.UnloadAssetLoader(handle);
            }
            else
            {
                Debug.LogError("AssetManager::InstantiateAsset->init is failed");
            }
        }
        
        public void DoUpdate(float deltaTime)
        {
            assetLoader?.DoUpdate(deltaTime);
        }
    }
}
