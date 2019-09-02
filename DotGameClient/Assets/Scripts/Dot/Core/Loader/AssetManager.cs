using Dot.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetManager : Singleton<AssetManager>
    {
        private AAssetLoader assetLoader = null;
        private bool isInit = false;
        public void InitLoader(AssetLoaderMode loaderMode,Action<bool> initCallback,params SystemObject[] sysObjs)
        {
            if(loaderMode == AssetLoaderMode.Resources)
            {
                assetLoader = new ResourceLoader();
            }else if(loaderMode == AssetLoaderMode.AssetDatabase)
            {
                assetLoader = new AssetDatabaseLoader();
            }else if(loaderMode == AssetLoaderMode.AssetBundle)
            {
                assetLoader = new AssetBundleLoader();
            }

            if(assetLoader!=null)
            {
                assetLoader.Initialize((isSuccess) =>
                {
                    isInit = isSuccess;
                    initCallback?.Invoke(isSuccess);
                }, sysObjs);
            }
        }

        public AssetLoaderHandle LoadAssetAsync(
            string assetPath,
            OnAssetLoadComplete complete, 
            AssetLoaderPriority priority = AssetLoaderPriority.Default,  
            OnAssetLoadProgress progress = null,
            SystemObject userData = null)
        {
            return assetLoader.LoadOrInstanceBatchAssetAsync(new string[] { assetPath }, false, priority, complete, progress, null, null, userData);
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
            return assetLoader.LoadOrInstanceBatchAssetAsync(assetPaths, false, priority, complete, progress, batchComplete, batchProgress, userData);
        }

        public AssetLoaderHandle InstanceAssetAsync(
            string assetPath,
            OnAssetLoadComplete complete,
            AssetLoaderPriority priority = AssetLoaderPriority.Default,
            OnAssetLoadProgress progress = null,
            SystemObject userData = null)
        {
            return assetLoader.LoadOrInstanceBatchAssetAsync(new string[] { assetPath }, true, priority, complete, progress, null, null, userData);
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
            return assetLoader.LoadOrInstanceBatchAssetAsync(assetPaths, true, priority, complete, progress, batchComplete, batchProgress, userData);
        }

        public void UnloadAssetLoader(AssetLoaderHandle handle)
        {

        }
        
        public void DoUpdate(float deltaTime)
        {
            assetLoader?.DoUpdate(deltaTime);
        }
    }
}
