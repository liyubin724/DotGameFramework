using Dot.Core.Pool;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class ResourceLoader : AAssetLoader
    {
        private readonly ObjectPool<ResourceLoaderData> loaderDataPool = new ObjectPool<ResourceLoaderData>(4);

        protected override AssetLoaderData GetLoaderData()=> loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as ResourceLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            ResourceLoaderData rLoaderData = loaderData as ResourceLoaderData;
            rLoaderData.Init();
            for(int i =0;i<rLoaderData.assetPaths.Length;++i)
            {
                ResourceAsyncOperation resourceAsyncOperation = new ResourceAsyncOperation(rLoaderData.assetPaths[i], GetAssetRootPath());
                asyncOperationORM.PushData(resourceAsyncOperation);

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
                ResourceAsyncOperation operation = rLoaderData.asyncOperations[i];
                string assetPath = rLoaderData.assetPaths[i];
                if(operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    UnityObject operationAsset = operation.GetAsset();
                    if(operationAsset == null)
                    {
                        operation.Status = AssetAsyncOperationStatus.Error;
                        rLoaderData.completeCallback?.Invoke(assetPath, null, rLoaderData.userData);
                    }else if(operationAsset !=null && loaderHandle.GetObject(i) == null)
                    {
                        UnityObject uObj = operationAsset;
                        if(rLoaderData.isInstance)
                        {
                            uObj = UnityObject.Instantiate(uObj);
                        }
                        loaderHandle.SetObject(i, uObj);
                        rLoaderData.completeCallback?.Invoke(assetPath, uObj, rLoaderData.userData);
                    }
                }else if(operation.Status == AssetAsyncOperationStatus.Loading)
                {
                    float oldProgress = loaderHandle.GetProgress(i);
                    float curProgress = operation.Progress();
                    if(oldProgress!=curProgress)
                    {
                        loaderHandle.SetProgress(i, curProgress);

                        rLoaderData.progressCallback?.Invoke(assetPath, curProgress);
                    }
                    isComplete = false;
                }else
                {
                    isComplete = false;
                }
            }

            rLoaderData.batchProgressCallback?.Invoke(rLoaderData.assetPaths, loaderHandle.Progresses());

            if(isComplete)
            {
                rLoaderData.batchCompleteCallback?.Invoke(rLoaderData.assetPaths, loaderHandle.GetObjects(), rLoaderData.userData);
            }
            return isComplete;
        }

        
    }

}
