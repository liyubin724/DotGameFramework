using Dot.Core.Pool;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetDatabaseLoader : AAssetLoader
    {
        private readonly ObjectPool<AssetDatabaseLoaderData> loaderDataPool = new ObjectPool<AssetDatabaseLoaderData>(4);
        protected override AssetLoaderData GetLoaderData() => loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetDatabaseLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            AssetDatabaseLoaderData rLoaderData = loaderData as AssetDatabaseLoaderData;
            rLoaderData.Init();
            for (int i = 0; i < rLoaderData.assetPaths.Length; ++i)
            {
                AssetDatabaseAsyncOperation operation = new AssetDatabaseAsyncOperation(rLoaderData.assetPaths[i]);
                asyncOperationORM.PushData(operation);

                rLoaderData.asyncOperations[i] = operation;
            }
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = true;
            return true;
        }

        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            AssetDatabaseLoaderData adLoaderData = loaderData as AssetDatabaseLoaderData;
            bool isComplete = true;
            for (int i = 0; i < adLoaderData.assetPaths.Length; ++i)
            {
                AssetDatabaseAsyncOperation operation = adLoaderData.asyncOperations[i];
                string assetPath = adLoaderData.assetPaths[i];
                if (operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    UnityObject operationAsset = operation.GetAsset();
                    if (operationAsset == null)
                    {
                        operation.Status = AssetAsyncOperationStatus.Error;
                        adLoaderData.completeCallback?.Invoke(assetPath, null, adLoaderData.userData);
                    }
                    else if (operationAsset != null && loaderHandle.GetObject(i) == null)
                    {
                        UnityObject uObj = operationAsset;
                        if (adLoaderData.isInstance)
                        {
                            uObj = UnityObject.Instantiate(uObj);
                        }
                        loaderHandle.SetObject(i, uObj);
                        adLoaderData.completeCallback?.Invoke(assetPath, uObj, adLoaderData.userData);
                    }
                }
                else if (operation.Status == AssetAsyncOperationStatus.Loading)
                {
                    float oldProgress = loaderHandle.GetProgress(i);
                    float curProgress = operation.Progress();
                    if (oldProgress != curProgress)
                    {
                        loaderHandle.SetProgress(i, curProgress);

                        adLoaderData.progressCallback?.Invoke(assetPath, curProgress);
                    }
                    isComplete = false;
                }
                else
                {
                    isComplete = false;
                }
            }

            adLoaderData.batchProgressCallback?.Invoke(adLoaderData.assetPaths, loaderHandle.Progresses());

            if (isComplete)
            {
                adLoaderData.batchCompleteCallback?.Invoke(adLoaderData.assetPaths, loaderHandle.Objects(), adLoaderData.userData);
            }
            return isComplete;
        }
    }
}
