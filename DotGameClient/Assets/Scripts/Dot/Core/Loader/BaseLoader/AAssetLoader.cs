using Dot.Core.Generic;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Loader
{
    public abstract class AAssetLoader
    {
        private UniqueIDCreator idCreator = new UniqueIDCreator();

        protected Dictionary<long, AssetLoaderData> loaderDataDic = new Dictionary<long, AssetLoaderData>();
        protected Dictionary<long, AssetLoaderHandle> loaderHandleDic = new Dictionary<long, AssetLoaderHandle>();

        protected FastPriorityQueue<AssetLoaderData> loaderDataWaitingQueue = new FastPriorityQueue<AssetLoaderData>(10);
        protected Dictionary<long, AssetLoaderData> loaderDataLoadingDic = new Dictionary<long, AssetLoaderData>();

        protected IndexMapORM<string, AAssetAsyncOperation> asyncOperationORM = new IndexMapORM<string, AAssetAsyncOperation>();
        public virtual int MaxLoadingCount { get; set; } = 5;

        private bool isInit = false;
        protected Action<bool> initCallback = null;
        public virtual void Initialize(Action<bool> initCallback, params SystemObject[] sysObjs)
        {
            this.initCallback = initCallback;
        }

        public AssetLoaderHandle LoadOrInstanceBatchAssetAsync(string[] assetPaths,
            bool isInstance,
            AssetLoaderPriority priority,
            OnAssetLoadComplete complete,
            OnAssetLoadProgress progress,
            OnBatchAssetLoadComplete batchComplete,
            OnBatchAssetsLoadProgress batchProgress,
            SystemObject userData)
        {
            long uniqueID = idCreator.Next();
            
            AssetLoaderData loaderData = GetLoaderData();
            loaderData.uniqueID = uniqueID;
            loaderData.assetPaths = assetPaths;
            loaderData.isInstance = isInstance;
            loaderData.completeCallback = complete;
            loaderData.progressCallback = progress;
            loaderData.batchCompleteCallback = batchComplete;
            loaderData.batchProgressCallback = batchProgress;
            loaderData.userData = userData;
            loaderData.InitData();

            loaderDataDic.Add(uniqueID, loaderData);
            if (loaderDataWaitingQueue.Count >= loaderDataWaitingQueue.MaxSize)
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
            if(!isInit)
            {
                CheckInitializeAction();
            }
            if(!isInit)
            {
                return;
            }

            if(loaderDataWaitingQueue.Count>0 && asyncOperationORM.Count<MaxLoadingCount)
            {
                AssetLoaderData loaderData = loaderDataWaitingQueue.Dequeue();
                loaderDataLoadingDic.Add(loaderData.uniqueID, loaderData);
                StartLoaderDataLoading(loaderData);
            }

            if(asyncOperationORM.Count > 0)
            {
                UpdateAsyncOperation();
            }

            if(loaderDataLoadingDic.Count>0)
            {
                UpdateLoadingLoaderData();
            }

            InnerDoUpdate();

            CheckUnloadUnusedAction();
        }

        protected virtual void InnerDoUpdate() { }

        private void UpdateLoadingLoaderData()
        {
            long[] uniqeIDs = loaderDataLoadingDic.Keys.ToArray();
            foreach(var uniqueID in uniqeIDs)
            {
                AssetLoaderData loaderData = loaderDataLoadingDic[uniqueID];
                AssetLoaderHandle loaderHandle = loaderHandleDic[uniqueID];
                if(UpdateLoadingLoaderData(loaderData,loaderHandle))
                {
                    loaderDataLoadingDic.Remove(uniqueID);
                    loaderHandleDic.Remove(uniqueID);
                    loaderDataDic.Remove(uniqueID);

                    ReleaseLoaderData(loaderData);
                }
            }
        }

        protected abstract bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle);

        private void UpdateAsyncOperation()
        {
            int index = 0;
            while(index< asyncOperationORM.Count && index<MaxLoadingCount)
            {
                AAssetAsyncOperation operation = asyncOperationORM.GetDataByIndex(index);
                operation.DoUpdate();
                if (operation.Status == AssetAsyncOperationStatus.None)
                {
                    operation.StartAsync();
                }else if(operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    asyncOperationORM.DeleteByIndex(index);
                    continue;
                }

                ++index;
            }
        }

        protected abstract AssetLoaderData GetLoaderData();
        protected abstract void ReleaseLoaderData(AssetLoaderData loaderData);
        protected abstract void StartLoaderDataLoading(AssetLoaderData loaderData);

        protected virtual string GetAssetRootPath()
        {
            return "";
        }

        public void UnloadAsset(AssetLoaderHandle handle)
        {

        }
        
        #region init asset Loader
        private void CheckInitializeAction()
        {
            if(UpdateInitialize(out bool isSuccess))
            {
                isInit = true;
                initCallback?.Invoke(isSuccess);
                initCallback = null;
            }
        }

        protected abstract bool UpdateInitialize(out bool isSuccess);
        #endregion

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
