using Dot.Core.Pool;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class ResourceLoader : AAssetLoader
    {
        private readonly ObjectPool<ResourceLoaderData> loaderDataPool = new ObjectPool<ResourceLoaderData>(4);
        private List<ResourceAsyncOperation> asyncOperations = new List<ResourceAsyncOperation>();
        protected override void DeleteAsyncOperation(int index)
        {
            asyncOperations.RemoveAt(index);
        }

        protected override AAssetAsyncOperation GetAsyncOperation(int index)
        {
            return asyncOperations[index];
        }

        protected override int GetAsyncOperationCount()
        {
            return asyncOperations.Count;
        }

        protected override AssetLoaderData GetLoaderData()=> loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as ResourceLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            ResourceLoaderData rLoaderData = loaderData as ResourceLoaderData;
            rLoaderData.InitData();
            for(int i =0;i<rLoaderData.assetPaths.Length;++i)
            {
                ResourceAsyncOperation resourceAsyncOperation = new ResourceAsyncOperation(rLoaderData.assetPaths[i], GetAssetRootPath());
                asyncOperations.Add(resourceAsyncOperation);

                rLoaderData.asyncOperations[i] = resourceAsyncOperation;
            }
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = true;
            return true;
        }

        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            ResourceLoaderData rLoaderData = loaderData as ResourceLoaderData;
            bool isComplete = true;
            for (int i = 0; i < rLoaderData.assetPaths.Length; ++i)
            {
                if(loaderData.GetIsCompleteCalled(i))
                {
                    continue;
                }
                string assetPath = rLoaderData.assetPaths[i];
                ResourceAsyncOperation operation = rLoaderData.asyncOperations[i];
                
                if (operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    UnityObject uObj = operation.GetAsset();
                    if (uObj!=null && rLoaderData.isInstance)
                    {
                        uObj = UnityObject.Instantiate(uObj);
                    }
                    loaderHandle.SetObject(i, uObj);
                    loaderHandle.SetProgress(i, 1.0f);

                    rLoaderData.InvokeProgress(i, 1.0f);
                    rLoaderData.InvokeComplete(i, uObj);
                }else if(operation.Status == AssetAsyncOperationStatus.Loading)
                {
                    float oldProgress = loaderHandle.GetProgress(i);
                    float curProgress = operation.Progress();
                    if(oldProgress!=curProgress)
                    {
                        loaderHandle.SetProgress(i, curProgress);
                        rLoaderData.InvokeProgress(i, curProgress);
                    }
                    isComplete = false;
                }else
                {
                    isComplete = false;
                }
            }

            rLoaderData.InvokeBatchProgress(loaderHandle.Progresses());

            if(isComplete)
            {
                rLoaderData.InvokeBatchComplete(loaderHandle.GetObjects());
            }
            return isComplete;
        }
    }
}
