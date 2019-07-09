using Priority_Queue;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public abstract class AAssetLoader
    {
        private int loaderIndex = 0;
        protected int GetLoaderIndex()
        {
            return ++loaderIndex;
        }

        protected Dictionary<int, AsyncData> assetAsyncDataDic = new Dictionary<int, AsyncData>();
        protected Dictionary<string, AsyncOperationData> allAsyncOperationDic = new Dictionary<string, AsyncOperationData>();
        protected FastPriorityQueue<AsyncOperationData> waitingOperationQueue = new FastPriorityQueue<AsyncOperationData>(200);
        protected List<AsyncOperationData> loadingOperationList = new List<AsyncOperationData>();
        
        protected abstract string GetAssetPath(string assetFullPath);
        public abstract UnityObject LoadAsset(string assetFullPath);
        public abstract UnityObject[] LoadBatchAsset(string[] assetFullPaths);
        public abstract int LoadAssetAsync(AssetAsyncData data);
        public abstract int LoadBatchAssetAsync(BatchAssetAsyncData data);
        public abstract void CancelAssetLoader(int index);
        public abstract void UnloadUnusedAsset();

        protected virtual int GetMaxLoadingCount()
        {
            return 3;
        }

        protected virtual int CachedMaxCount()
        {
            return 50;
        }

        protected virtual float ClearInterval()
        {
            return 180;
        }

        private List<int> finishDataList = new List<int>();
        private List<string> clearPathList = new List<string>();
        private float curClearTime = 0.0f;
        public virtual void DoUpdate(float deltaTime)
        {
            if(assetAsyncDataDic.Count>0)
            {
                foreach (var kvp in assetAsyncDataDic)
                {
                    if (kvp.Value.UpdateOperation())
                    {
                        finishDataList.Add(kvp.Key);
                        kvp.Value.Finish();
                    }
                }
                foreach (var i in finishDataList)
                {
                    assetAsyncDataDic.Remove(i);
                }
                finishDataList.Clear();
            }

            if (loadingOperationList.Count > 0)
            {
                for (int i = loadingOperationList.Count - 1; i >= 0; --i)
                {
                    if (loadingOperationList[i].IsDone)
                    {
                        loadingOperationList.RemoveAt(i);
                    }
                }
            }

            int canStartCount = GetMaxLoadingCount() - loadingOperationList.Count;
            if (canStartCount > 0 && waitingOperationQueue.Count > 0)
            {
                for (int i = 0; i < canStartCount; i++)
                {
                    if (waitingOperationQueue.Count == 0)
                        break;
                    AsyncOperationData aoData = waitingOperationQueue.Dequeue();
                    loadingOperationList.Add(aoData);
                    aoData.StartLoad();
                }
            }

            if(allAsyncOperationDic.Count>0)
            {
                curClearTime += deltaTime;
                if (curClearTime >= ClearInterval() || allAsyncOperationDic.Count > CachedMaxCount())
                {
                    curClearTime = 0.0f;

                    foreach (var kvp in allAsyncOperationDic)
                    {
                        if (kvp.Value.RetainCount == 0)
                        {
                            clearPathList.Add(kvp.Key);
                        }
                    }
                    foreach (var path in clearPathList)
                    {
                        allAsyncOperationDic.Remove(path);
                    }
                    clearPathList.Clear();
                }
            }
            
        }
    }
}
