using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetDatabaseLoader : AAssetLoader
    {
        private readonly ObjectPool<AssetDatabaseLoaderData> loaderDataPool = new ObjectPool<AssetDatabaseLoaderData>(4);
        private List<AssetDatabaseAsyncOperation> asyncOperations = new List<AssetDatabaseAsyncOperation>();
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

        protected override AssetLoaderData GetLoaderData() => loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetDatabaseLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            AssetDatabaseLoaderData rLoaderData = loaderData as AssetDatabaseLoaderData;
            rLoaderData.Init();
            for (int i = 0; i < rLoaderData.assetPaths.Length; ++i)
            {
                AssetDatabaseAsyncOperation operation = new AssetDatabaseAsyncOperation(rLoaderData.assetPaths[i], GetAssetRootPath());
                asyncOperations.Add(operation);

                rLoaderData.asyncOperations[i] = operation;
            }
        }

        public override void Initialize(AssetPathMode pathMode, Action<bool> initCallback, params object[] sysObjs)
        {
            base.Initialize(pathMode, initCallback, sysObjs);
#if UNITY_EDITOR
            assetAddressConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetAddressConfig>(AssetAddressConfig.CONFIG_PATH);
#endif
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
                if (loaderData.GetIsCompleteCalled(i))
                {
                    continue;
                }
                string assetPath = adLoaderData.assetPaths[i];
                AssetDatabaseAsyncOperation operation = adLoaderData.asyncOperations[i];
                
                if (operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    UnityObject uObj = operation.GetAsset();
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

            adLoaderData.InvokeBatchProgress(loaderHandle.Progresses());

            if (isComplete)
            {
                adLoaderData.InvokeBatchComplete(loaderHandle.GetObjects());
            }
            return isComplete;
        }
    }
}
