using Dot.Core.Generic;
using Priority_Queue;
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

        protected Dictionary<long, AssetLoaderData> loaderDataDic = new Dictionary<long, AssetLoaderData>();
        protected Dictionary<long, AssetLoaderHandle> loaderHandleDic = new Dictionary<long, AssetLoaderHandle>();

        protected FastPriorityQueue<AssetLoaderData> loaderDataWaitingQueue = new FastPriorityQueue<AssetLoaderData>(10);
        protected Dictionary<long, AssetLoaderData> loaderDataLoadingDic = new Dictionary<long, AssetLoaderData>();
        
        protected IndexMapORM<string, AAssetAsyncOperation> asyncOperations = new IndexMapORM<string, AAssetAsyncOperation>();

        public virtual int MaxLoadingCount { get; set; } = 5;
        public abstract void Initialize(Action<bool> initCallback);
        
        private AssetLoaderHandle LoadOrInstanceBatchAssetAsync(string[] assetPaths,
            bool isInstance,
            AssetLoaderPriority priority,
            OnAssetLoadComplete complete,
            OnAssetLoadProgress progress,
            OnBatchAssetLoadComplete batchComplete,
            OnBatchAssetsLoadProgress batchProgress,
            SystemObject userData)
        {
            long uniqueID = idCreator.Next();

            AssetLoaderData loaderData = new AssetLoaderData
            {
                uniqueID = uniqueID,
                assetPaths = assetPaths,
                isInstance = isInstance,
                completeCallback = complete,
                progressCallback = progress,
                batchCompleteCallback = batchComplete,
                batchProgressCallback = batchProgress,
                userData = userData
            };

            loaderDataDic.Add(uniqueID, loaderData);
            if(loaderDataWaitingQueue.Count >= loaderDataWaitingQueue.MaxSize)
            {
                loaderDataWaitingQueue.Resize(loaderDataWaitingQueue.MaxSize * 2);
            }
            loaderDataWaitingQueue.Enqueue(loaderData, (float)priority);

            AssetLoaderHandle handle = new AssetLoaderHandle(uniqueID, assetPaths);
            loaderHandleDic.Add(uniqueID, handle);

            return handle;
        }
        
        public void DoUpdate(float deltaTime)
        {

            CheckUnloadUnusedAction();
        }

        public void UnloadAsset(AssetLoaderHandle handle)
        {

        }

        #region unloadUnusedAsset
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
        #endregion
    }
}
