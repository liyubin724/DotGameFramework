using Dot.Core.Util;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetManager : Singleton<AssetManager>
    {
        private AAssetLoader assetLoader = null;
        private SceneAssetLoader sceneLoader = null;
        private bool isInit = false;
        public void InitLoader(AssetLoaderMode loaderMode, AssetPathMode pathMode, int maxLoadingCount, string assetRootDir, Action<bool> initCallback)
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
            assetLoader?.Initialize((isSuccess) =>
            {
                isInit = isSuccess;
                initCallback?.Invoke(isSuccess);

                sceneLoader = new SceneAssetLoader(loaderMode, assetLoader);

            },pathMode,maxLoadingCount,assetRootDir);
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
                if(string.IsNullOrEmpty(assetPath) || asset == null)
                {
                    Debug.LogError($"AssetManager::InstantiateAsset->asset is null or asset is null.assetPath = {(assetPath ?? "")}");
                    return null;
                }
                return assetLoader?.InstantiateAsset(assetPath, asset);
            }
            else
            {
                Debug.LogError("AssetManager::InstantiateAsset->init is failed");
                return null;
            }
        }

        public SceneLoaderHandle LoadSceneAsync(string pathOrAddress,
            OnSceneComplete completeCallback,
            OnSceneProgress progressCallback,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            bool activateOnLoad = true,
            SystemObject userData = null)
        {
            return sceneLoader.LoadSceneAsync(pathOrAddress, completeCallback, progressCallback, loadMode, activateOnLoad, userData);
        }

        public void UnloadSceneAsync(string pathOrAddress,
            OnSceneComplete completeCallback,
            OnSceneProgress progressCallback,
            SystemObject userData = null)
        {
            sceneLoader.UnloadSceneAsync(pathOrAddress, completeCallback, progressCallback, userData);
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

        public void UnloadAssetLoader(AssetLoaderHandle handle, bool destroyIfLoaded = false)
        {
            if (isInit)
            {
               assetLoader?.UnloadAssetLoader(handle, destroyIfLoaded);
            }
            else
            {
                Debug.LogError("AssetManager::InstantiateAsset->init is failed");
            }
        }
        
        public void DoUpdate(float deltaTime)
        {
            assetLoader?.DoUpdate(deltaTime);
            sceneLoader?.DoUpdate(deltaTime);
        }
    }
}
