using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetDatabaseLoader : AAssetLoader
    {
        protected override void InnerInitialize(string assetRootDir)
        {
#if UNITY_EDITOR
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
        
        private Dictionary<long, List<AssetDatabaseAsyncOperation>> asyncOperationDic = new Dictionary<long, List<AssetDatabaseAsyncOperation>>();
        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            List<AssetDatabaseAsyncOperation> operationList = new List<AssetDatabaseAsyncOperation>();
            asyncOperationDic.Add(loaderData.uniqueID, operationList);
            for (int i = 0; i < loaderData.assetPaths.Length; ++i)
            {
                AssetDatabaseAsyncOperation operation = new AssetDatabaseAsyncOperation(loaderData.assetPaths[i]);
                loadingAsyncOperationList.Add(operation);
                operationList.Add(operation);
            }
        }
        
        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            List<AssetDatabaseAsyncOperation> operationList = asyncOperationDic[loaderData.uniqueID];
            bool isComplete = true;
            for (int i = 0; i < loaderData.assetPaths.Length; ++i)
            {
                if(loaderHandle.GetAssetState(i))
                {
                    continue;
                }
                string assetPath = loaderData.assetPaths[i];
                AssetDatabaseAsyncOperation operation = operationList[i];

                if (operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    UnityObject uObj = operation.GetAsset();

                    if(uObj == null)
                    {
                        Debug.LogError($"AssetDatabaseLoader::UpdateLoadingLoaderData->asset is null.path = {assetPath}");
                    }

                    if (uObj != null && loaderData.isInstance)
                    {
                        uObj = UnityObject.Instantiate(uObj);
                    }
                    loaderHandle.SetObject(i, uObj);
                    loaderHandle.SetProgress(i, 1.0f);

                    loaderData.InvokeProgress(i, 1.0f);
                    loaderData.InvokeComplete(i, uObj);
                }
                else if (operation.Status == AssetAsyncOperationStatus.Loading)
                {
                    float oldProgress = loaderHandle.GetProgress(i);
                    float curProgress = operation.Progress();
                    if (oldProgress != curProgress)
                    {
                        loaderHandle.SetProgress(i, curProgress);
                        loaderData.InvokeProgress(i, curProgress);
                    }
                    isComplete = false;
                }
                else
                {
                    isComplete = false;
                }
            }

            loaderData.InvokeBatchProgress(loaderHandle.AssetProgresses);

            if (isComplete)
            {
                loaderData.InvokeBatchComplete(loaderHandle.AssetObjects);
                asyncOperationDic.Remove(loaderData.uniqueID);
            }
            return isComplete;
        }

        protected override void UnloadLoadingAssetLoader(AssetLoaderData loaderData, AssetLoaderHandle handle, bool destroyIfLoaded)
        {
            //for(int i =0;i< loaderData.assetPaths.Length;++i)
            //{
            //    if(handle.GetAssetState(i))
            //    {
            //        if(loaderData.isInstance && destroyIfLoaded)
            //        {
            //            UnityObject uObj = handle.GetObject(i);
            //            if (uObj != null)
            //            {
            //                UnityObject.Destroy(uObj);
            //            }
            //        }
            //    }else
            //    {
            //        AssetDatabaseAsyncOperation operation = loaderData.asyncOperations[i];
            //        loadingAsyncOperationList.Remove(operation);
            //    }
            //}
        }
    }
}
