using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetDatabaseLoader : AAssetLoader
    {
        private AssetAddressConfig assetAddressConfig = null;
        private AssetPathMode pathMode = AssetPathMode.Address;
        protected override void InnerInitialize(AssetPathMode pathMode, string assetRootDir)
        {
#if UNITY_EDITOR
            this.pathMode = pathMode;
            if(pathMode == AssetPathMode.Address)
            {
                assetAddressConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetAddressConfig>(AssetAddressConfig.CONFIG_PATH);
            }
#else
            Debug.LogError("");
#endif
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = true;
            if(pathMode == AssetPathMode.Address && assetAddressConfig == null)
            {
                isSuccess = false;
            }
            return true;
        }

        private readonly ObjectPool<AssetDatabaseLoaderData> loaderDataPool = new ObjectPool<AssetDatabaseLoaderData>(4);
        protected override AssetLoaderData GetLoaderData(string[] assetPaths)
        {
            AssetDatabaseLoaderData loaderData = loaderDataPool.Get();

            if (pathMode == AssetPathMode.Address)
            {
                loaderData.assetAddresses = assetPaths;
                loaderData.assetPaths = assetAddressConfig.GetAssetPathByAddress(assetPaths);
            }
            else
            {
                loaderData.assetPaths = assetPaths;
            }
            loaderData.InitData(pathMode);
            return loaderData;
        }

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetDatabaseLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            AssetDatabaseLoaderData rLoaderData = loaderData as AssetDatabaseLoaderData;
            for (int i = 0; i < rLoaderData.assetPaths.Length; ++i)
            {
                AssetDatabaseAsyncOperation operation = new AssetDatabaseAsyncOperation(rLoaderData.assetPaths[i]);
                loadingAsyncOperationList.Add(operation);

                rLoaderData.asyncOperations[i] = operation;
            }
        }
        
        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            AssetDatabaseLoaderData adLoaderData = loaderData as AssetDatabaseLoaderData;
            
            bool isComplete = true;
            for (int i = 0; i < adLoaderData.assetPaths.Length; ++i)
            {
                if(loaderHandle.GetAssetState(i))
                {
                    continue;
                }
                string assetPath = adLoaderData.assetPaths[i];
                AssetDatabaseAsyncOperation operation = adLoaderData.asyncOperations[i];
                
                if (operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    UnityObject uObj = operation.GetAsset();

                    if(uObj == null)
                    {
                        Debug.LogError($"AssetDatabaseLoader::UpdateLoadingLoaderData->asset is null.path = {assetPath}");
                    }

                    if (uObj != null && adLoaderData.isInstance)
                    {
                        uObj = UnityObject.Instantiate(uObj);
                    }
                    loaderHandle.SetObject(i, uObj);
                    loaderHandle.SetProgress(i, 1.0f);

                    adLoaderData.InvokeProgress(i, 1.0f);
                    adLoaderData.InvokeComplete(i, uObj);
                }
                else if (operation.Status == AssetAsyncOperationStatus.Loading)
                {
                    float oldProgress = loaderHandle.GetProgress(i);
                    float curProgress = operation.Progress();
                    if (oldProgress != curProgress)
                    {
                        loaderHandle.SetProgress(i, curProgress);
                        adLoaderData.InvokeProgress(i, curProgress);
                    }
                    isComplete = false;
                }
                else
                {
                    isComplete = false;
                }
            }

            adLoaderData.InvokeBatchProgress(loaderHandle.AssetProgresses);

            if (isComplete)
            {
                adLoaderData.InvokeBatchComplete(loaderHandle.AssetObjects);
            }
            return isComplete;
        }

        protected override void UnloadLoadingAssetLoader(AssetLoaderData loaderData, AssetLoaderHandle handle, bool destroyIfLoaded)
        {
            AssetDatabaseLoaderData adLoaderData = loaderData as AssetDatabaseLoaderData;
            for(int i =0;i<adLoaderData.assetPaths.Length;++i)
            {
                if(handle.GetAssetState(i))
                {
                    if(loaderData.isInstance && destroyIfLoaded)
                    {
                        UnityObject uObj = handle.GetObject(i);
                        if (uObj != null)
                        {
                            UnityObject.Destroy(uObj);
                        }
                    }
                }else
                {
                    AssetDatabaseAsyncOperation operation = adLoaderData.asyncOperations[i];
                    loadingAsyncOperationList.Remove(operation);
                }
            }
        }
    }
}
