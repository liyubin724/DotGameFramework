using Dot.Core.Generic;
using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public abstract class AAssetLoader
    {
        private UniqueIDCreator idCreator = new UniqueIDCreator();

        protected ObjectPool<AssetLoaderData> loaderDataPool = new ObjectPool<AssetLoaderData>(10);
        protected FastPriorityQueue<AssetLoaderData> loaderDataWaitingQueue = new FastPriorityQueue<AssetLoaderData>(10);
        protected List<AssetLoaderData> loaderDataLoadingList = new List<AssetLoaderData>();

        protected Dictionary<long, AssetLoaderHandle> loaderHandleDic = new Dictionary<long, AssetLoaderHandle>();

        protected List<AAssetAsyncOperation> loadingAsyncOperationList = new List<AAssetAsyncOperation>();

        #region init Loader
        private bool isInitFinished = false;
        private bool isInitSuccess = false;

        protected AssetPathMode pathMode = AssetPathMode.Address;
        protected AssetAddressConfig assetAddressConfig = null;

        internal string GetAssetPath(string pathOrAddress)
        {
            if(pathMode == AssetPathMode.Address)
            {
                return assetAddressConfig.GetAssetPathByAddress(pathOrAddress);
            }
            return pathOrAddress;
        }

        protected Action<bool> initCallback = null;
        private int maxLoadingCount = 5;
        public void Initialize(Action<bool> initCallback,AssetPathMode pathMode,int maxLoadingCount,string assetRootDir)
        {
            this.initCallback = initCallback;
            this.maxLoadingCount = maxLoadingCount;
            this.pathMode = pathMode;

            InnerInitialize(assetRootDir);
        }
        protected abstract void InnerInitialize(string assetRootDir);
        protected abstract bool UpdateInitialize(out bool isSuccess);
        #endregion


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

            if(assetPaths == null || assetPaths.Length == 0)
            {
                Debug.LogError($"AssetLoader::LoadOrInstanceBatchAssetAsync->assetPaths is Null");
                return null;
            }

            AssetLoaderData loaderData = loaderDataPool.Get();
            if (pathMode == AssetPathMode.Address)
            {
                loaderData.assetAddresses = assetPaths;
                loaderData.assetPaths = assetAddressConfig.GetAssetPathByAddress(assetPaths);
                if (loaderData.assetPaths == null)
                {
                    loaderDataPool.Release(loaderData);
                    Debug.LogError($"ResourceLoader::GetLoaderData->asset not found.address = {string.Join(",", assetPaths)}");
                    return null;
                }
            }
            else
            {
                loaderData.assetPaths = assetPaths;
            }
            loaderData.InitData(pathMode);

            loaderData.uniqueID = uniqueID;
            loaderData.isInstance = isInstance;
            loaderData.completeCallback = complete;
            loaderData.progressCallback = progress;
            loaderData.batchCompleteCallback = batchComplete;
            loaderData.batchProgressCallback = batchProgress;
            loaderData.userData = userData;

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
            if(!isInitFinished)
            {
                isInitFinished = UpdateInitialize(out isInitSuccess);
                if(isInitFinished)
                {
                    if(!isInitSuccess)
                    {
                        Debug.LogError("AssetLoader::DoUpdate->init failed");
                    }

                    initCallback?.Invoke(isInitSuccess);
                    initCallback = null;
                }
                return;
            }
            if(!isInitSuccess)
            {
                return;
            }

            UpdateWaitingLoaderData();
            UpdateAsyncOperation();
            UpdateLoadingLoaderData();

            CheckUnloadUnusedAction();
        }

        private void UpdateWaitingLoaderData()
        {
            while (loaderDataWaitingQueue.Count > 0 && loadingAsyncOperationList.Count < maxLoadingCount)
            {
                AssetLoaderData loaderData = loaderDataWaitingQueue.Dequeue();
                loaderDataLoadingList.Add(loaderData);
                StartLoaderDataLoading(loaderData);
            }
        }

        private void UpdateAsyncOperation()
        {
            if(loadingAsyncOperationList.Count>0)
            {
                int index = 0;
                bool isOperationLoaded = false;
                while(index<loadingAsyncOperationList.Count&&index<maxLoadingCount)
                {
                    AAssetAsyncOperation operation = loadingAsyncOperationList[index];
                    operation.DoUpdate();

                    if (operation.Status == AssetAsyncOperationStatus.None)
                    {
                        operation.StartAsync();
                    }
                    else if (operation.Status == AssetAsyncOperationStatus.Loaded)
                    {
                        loadingAsyncOperationList.RemoveAt(index);
                        isOperationLoaded = true;
                        continue;
                    }

                    ++index;
                }
                if(isOperationLoaded)
                {
                    OnAsyncOperationLoaded();
                }
            }
        }

        protected virtual void OnAsyncOperationLoaded()
        {

        }

        private void UpdateLoadingLoaderData()
        {
            if(loaderDataLoadingList.Count>0)
            {
                for(int i = loaderDataLoadingList.Count-1;i>=0;--i)
                {
                    AssetLoaderData loaderData = loaderDataLoadingList[i];
                    if (UpdateLoadingLoaderData(loaderData))
                    {
                        loaderDataLoadingList.RemoveAt(i);
                        loaderHandleDic.Remove(loaderData.uniqueID);
                        loaderDataPool.Release(loaderData);
                    }
                }
            }
        }

        protected abstract bool UpdateLoadingLoaderData(AssetLoaderData loaderData);
        
        protected abstract void StartLoaderDataLoading(AssetLoaderData loaderData);

        #region unload Asset
        /// <summary>
        /// 
        /// 警告：在AssetBundle中，会强制性的卸载掉使用的资源，请确保指向的资源没有其它使用者。
        /// </summary>
        /// <param name="pathOrAddress"></param>
        public virtual void UnloadAsset(string pathOrAddress)
        {

        }

        public void UnloadAssetLoader(AssetLoaderHandle handle,bool destroyIfLoaded)
        {
            if(loaderHandleDic.ContainsKey(handle.UniqueID))
            {
                loaderHandleDic.Remove(handle.UniqueID);
            }else
            {
                return;
            }
            
            AssetLoaderData loaderData = null;
            foreach(var data in loaderDataWaitingQueue)
            {
                if(data.uniqueID == handle.UniqueID)
                {
                    loaderData = data;
                    break;
                }
            }
            if(loaderData!=null)
            {
                handle.BreakLoader(loaderData.isInstance && destroyIfLoaded);
                loaderDataWaitingQueue.Remove(loaderData);
                loaderDataPool.Release(loaderData);
                return;
            }
            foreach(var data in loaderDataLoadingList)
            {
                if(data.uniqueID == handle.UniqueID)
                {
                    loaderData = data;
                    break;
                }
            }
            if(loaderData!=null)
            {
                handle.BreakLoader(loaderData.isInstance && destroyIfLoaded);
                UnloadLoadingAssetLoader(loaderData);
            }
        }

        protected abstract void UnloadLoadingAssetLoader(AssetLoaderData loaderData);

        private void CheckUnloadUnusedAction()
        {
            if(unloadUnusedAssetOperation!=null)
            {
                if(unloadUnusedAssetOperation.isDone)
                {
                    InnerUnloadUnusedAssets();

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
                unloadUnusedAssetCallback = callback;

                GC.Collect();
                unloadUnusedAssetOperation = Resources.UnloadUnusedAssets();
                GC.Collect();
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
