using Dot.Core.Generic;
using Dot.Core.Loader.Config;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public abstract class AAssetLoader
    {
        private UniqueIDCreator idCreator = new UniqueIDCreator();
        protected AssetAddressConfig assetAddressConfig = null;

        protected Dictionary<long, AssetLoaderData> loaderDataDic = new Dictionary<long, AssetLoaderData>();
        protected Dictionary<long, AssetLoaderHandle> loaderHandleDic = new Dictionary<long, AssetLoaderHandle>();

        protected FastPriorityQueue<AssetLoaderData> loaderDataWaitingQueue = new FastPriorityQueue<AssetLoaderData>(10);
        protected Dictionary<long, AssetLoaderData> loaderDataLoadingDic = new Dictionary<long, AssetLoaderData>();

        public virtual int MaxLoadingCount { get; set; } = 5;

        private bool isInit = false;
        protected Action<bool> initCallback = null;
        protected AssetPathMode pathMode = AssetPathMode.Path;
        public virtual void Initialize(AssetPathMode pathMode,Action<bool> initCallback, params SystemObject[] sysObjs)
        {
            this.pathMode = pathMode;
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
            if(pathMode == AssetPathMode.Address)
            {
                loaderData.assetAddresses = assetPaths;
                loaderData.assetPaths = assetAddressConfig.GetAssetPathByAddress(assetPaths);
            }
            else
            {
                loaderData.assetPaths = assetPaths;
            }
            loaderData.isInstance = isInstance;
            loaderData.completeCallback = complete;
            loaderData.progressCallback = progress;
            loaderData.batchCompleteCallback = batchComplete;
            loaderData.batchProgressCallback = batchProgress;
            loaderData.userData = userData;
            loaderData.InitData(pathMode);

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

            if(loaderDataWaitingQueue.Count>0 && GetAsyncOperationCount() < MaxLoadingCount)
            {
                AssetLoaderData loaderData = loaderDataWaitingQueue.Dequeue();
                loaderDataLoadingDic.Add(loaderData.uniqueID, loaderData);
                StartLoaderDataLoading(loaderData);
            }

            if(GetAsyncOperationCount() > 0)
            {
                UpdateAsyncOperation();
            }

            if(loaderDataLoadingDic.Count>0)
            {
                UpdateLoadingLoaderData();
            }

            CheckUnloadUnusedAction();
        }

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
            while(index< GetAsyncOperationCount() && index<MaxLoadingCount)
            {
                AAssetAsyncOperation operation = GetAsyncOperation(index);
                operation.DoUpdate();
                if (operation.Status == AssetAsyncOperationStatus.None)
                {
                    operation.StartAsync();
                }else if(operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    DeleteAsyncOperation(index);
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

        public void UnloadAssetLoader(AssetLoaderHandle handle)
        {
            if(loaderDataDic.TryGetValue(handle.UniqueID,out AssetLoaderData loaderData))
            {
                if(loaderDataWaitingQueue.Contains(loaderData))
                {
                    loaderDataWaitingQueue.Remove(loaderData);
                    loaderDataDic.Remove(handle.UniqueID);
                    loaderHandleDic.Remove(handle.UniqueID);
                    return;
                }



            }
        }

        #region Async Operation

        protected abstract int GetAsyncOperationCount();
        protected abstract AAssetAsyncOperation GetAsyncOperation(int index);
        protected abstract void DeleteAsyncOperation(int index);

        #endregion


        #region init Loader
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
                GC.Collect();
                InnerUnloadUnusedAssets();
                unloadUnusedAssetCallback = callback;
            }
        }

        protected virtual void InnerUnloadUnusedAssets() { }

        #endregion

        public virtual UnityObject InstantiateAsset(string assetPath, UnityObject asset)
        {
            return UnityObject.Instantiate(asset);
        }
    }
}
