using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class ResourceLoader : AAssetLoader
    {
        private AssetAddressConfig assetAddressConfig = null;
        private AssetPathMode pathMode = AssetPathMode.Address;

        protected override void InnerInitialize(AssetPathMode pathMode, string assetRootDir)
        {
            this.pathMode = pathMode;
            if (pathMode == AssetPathMode.Address)
            {
                assetAddressConfig = Resources.Load<AssetAddressConfig>(AssetAddressConfig.CONFIG_PATH);
                
            }
        }
        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = true;
            if (pathMode == AssetPathMode.Address && assetAddressConfig == null)
            {
                isSuccess = false;
            }
            return true;
        }

        private readonly ObjectPool<ResourceLoaderData> loaderDataPool = new ObjectPool<ResourceLoaderData>(4);
        protected override AssetLoaderData GetLoaderData(string[] assetPaths)
        {
            ResourceLoaderData loaderData = loaderDataPool.Get();

            if (pathMode == AssetPathMode.Address)
            {
                loaderData.assetAddresses = assetPaths;
                loaderData.assetPaths = assetAddressConfig.GetAssetPathByAddress(assetPaths);
                if (loaderData.assetPaths == null)
                {
                    ReleaseLoaderData(loaderData);
                    Debug.LogError($"ResourceLoader::GetLoaderData->asset not found.address = {string.Join(",", assetPaths)}");
                    return null;
                }
            }
            else
            {
                loaderData.assetPaths = assetPaths;
            }
            loaderData.InitData(pathMode);

            return loaderData;
        }

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as ResourceLoaderData);


        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            ResourceLoaderData rLoaderData = loaderData as ResourceLoaderData;
            rLoaderData.InitData(pathMode);
            for (int i = 0; i < rLoaderData.assetPaths.Length; ++i)
            {
                ResourceAsyncOperation resourceAsyncOperation = new ResourceAsyncOperation(rLoaderData.assetPaths[i]);
                loadingAsyncOperationList.Add(resourceAsyncOperation);

                rLoaderData.asyncOperations[i] = resourceAsyncOperation;
            }
        }
        
        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            ResourceLoaderData rLoaderData = loaderData as ResourceLoaderData;
            bool isComplete = true;
            for (int i = 0; i < rLoaderData.assetPaths.Length; ++i)
            {
                if (loaderHandle.GetAssetState(i))
                {
                    continue;
                }
                string assetPath = rLoaderData.assetPaths[i];
                ResourceAsyncOperation operation = rLoaderData.asyncOperations[i];

                if (operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    UnityObject uObj = operation.GetAsset();

                    if (uObj == null)
                    {
                        Debug.LogError($"AssetDatabaseLoader::UpdateLoadingLoaderData->asset is null.path = {assetPath}");
                    }

                    if (uObj != null && rLoaderData.isInstance)
                    {
                        uObj = UnityObject.Instantiate(uObj);
                    }
                    loaderHandle.SetObject(i, uObj);
                    loaderHandle.SetProgress(i, 1.0f);

                    rLoaderData.InvokeProgress(i, 1.0f);
                    rLoaderData.InvokeComplete(i, uObj);
                }
                else if (operation.Status == AssetAsyncOperationStatus.Loading)
                {
                    float oldProgress = loaderHandle.GetProgress(i);
                    float curProgress = operation.Progress();
                    if (oldProgress != curProgress)
                    {
                        loaderHandle.SetProgress(i, curProgress);
                        rLoaderData.InvokeProgress(i, curProgress);
                    }
                    isComplete = false;
                }
                else
                {
                    isComplete = false;
                }
            }

            rLoaderData.InvokeBatchProgress(loaderHandle.AssetProgresses);

            if (isComplete)
            {
                rLoaderData.InvokeBatchComplete(loaderHandle.AssetObjects);
            }
            return isComplete;
        }

        protected override void UnloadLoadingAssetLoader(AssetLoaderData loaderData, AssetLoaderHandle handle, bool destroyIfLoaded)
        {
            ResourceLoaderData rLoaderData = loaderData as ResourceLoaderData;
            for (int i = 0; i < rLoaderData.assetPaths.Length; ++i)
            {
                if (handle.GetAssetState(i))
                {
                    if (loaderData.isInstance && destroyIfLoaded)
                    {
                        UnityObject uObj = handle.GetObject(i);
                        if (uObj != null)
                        {
                            UnityObject.Destroy(uObj);
                        }
                    }
                }
                else
                {
                    ResourceAsyncOperation operation = rLoaderData.asyncOperations[i];
                    loadingAsyncOperationList.Remove(operation);
                }
            }
        }

    }
}
