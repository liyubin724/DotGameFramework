using Dot.Core.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public abstract class AAssetLoader
    {
        private UniqueIDCreator idCreator = new UniqueIDCreator();

        private Dictionary<long, AssetLoaderData> waitingLoaderDataDic = new Dictionary<long, AssetLoaderData>();
        private Dictionary<long, AssetLoaderData> loadingLoaderDataDic = new Dictionary<long, AssetLoaderData>();

        private Dictionary<long, AssetLoaderHandle> loaderHandleDic = new Dictionary<long, AssetLoaderHandle>();

        private IndexMapORM<string, AAssetAsyncOperation> asyncOperations = new IndexMapORM<string, AAssetAsyncOperation>();

        public virtual int MaxLoadingCount { get; set; } = 5;
        public abstract void Initialize(Action<bool> initCallback);
        
        private AssetLoaderHandle LoadOrInstanceBatchAssetAsync(string[] assetPaths,
            bool isInstance,
            OnAssetLoadComplete complete,
            OnAssetLoadProgress progress,
            OnBatchAssetLoadComplete batchComplete,
            OnBatchAssetsLoadProgress batchProgress,
            SystemObject userData)
        {
            long uniqueID = idCreator.Next();

            AssetLoaderData loaderData = new AssetLoaderData();
            loaderData.uniqueID = uniqueID;
            loaderData.assetPaths = assetPaths;
            loaderData.isInstance = isInstance;
            loaderData.completeCallback = complete;
            loaderData.progressCallback = progress;
            loaderData.batchCompleteCallback = batchComplete;
            loaderData.batchProgressCallback = batchProgress;
            loaderData.userData = userData;

            waitingLoaderDataDic.Add(uniqueID, loaderData);

            AssetLoaderHandle handle = new AssetLoaderHandle(uniqueID, assetPaths);
            loaderHandleDic.Add(uniqueID, handle);

            return handle;
        }


        
        public void DoUpdate(float deltaTime)
        {

            CheckUnloadUnusedAction();
        }

        private void CheckUnloadUnusedAction()
        {
            if(unloadUnusedAssetOperation!=null)
            {
                if(unloadUnusedAssetOperation.isDone)
                {
                    unloadUnusedAssetOperation = null;
                    unloadUnusedAssetCallback?.Invoke();
                    unloadUnusedAssetCallback = null;
                }
            }
        }

        private AsyncOperation unloadUnusedAssetOperation = null;
        private Action unloadUnusedAssetCallback = null;
        public void UnloadUnusedAssets(Action callback)
        {
            if(unloadUnusedAssetOperation == null)
            {
                GC.Collect();
                unloadUnusedAssetOperation = Resources.UnloadUnusedAssets();
                unloadUnusedAssetCallback = callback;
            }
        }

    }
}
