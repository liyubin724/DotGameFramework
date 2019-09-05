using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public static class AssetBundleConst
    {
        public static readonly string ASSETBUNDLE_MAINFEST_NAME = "assetbundles";
    }

    public class AssetBundleLoader : AAssetLoader
    {
        private string assetBundleRootPath = "";
        private AssetBundleManifest assetBundleManifest = null;
        private AssetInBundleConfig assetInBundleConfig = null;

        private readonly ObjectPool<AssetBundleLoaderData> loaderDataPool = new ObjectPool<AssetBundleLoaderData>(5);
        private readonly ObjectPool<AssetNode> assetNodePool = new ObjectPool<AssetNode>(50);
        private readonly ObjectPool<BundleNode> bundleNodePool = new ObjectPool<BundleNode>(50);

        private Dictionary<string, AssetNode> assetNodeDic = new Dictionary<string, AssetNode>();
        private Dictionary<string, BundleNode> bundleNodeDic = new Dictionary<string, BundleNode>();

        protected override AssetLoaderData GetLoaderData() => loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetBundleLoaderData);

        public override void Initialize(Action<bool> initCallback, params object[] sysObjs)
        {
            base.Initialize(initCallback, sysObjs);
            assetBundleRootPath = sysObjs[0] as string;

            string manifestPath = $"{assetBundleRootPath}/{AssetBundleConst.ASSETBUNDLE_MAINFEST_NAME}";
            AssetBundle manifestAB = AssetBundle.LoadFromFile(manifestPath);
            assetBundleManifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //manifestAB.Unload(false);

            string assetInBundlePath = $"{assetBundleRootPath}/{AssetInBundleConfig.CONFIG_ASSET_BUNDLE_NAME}";
            AssetBundle assetInBundleAB = AssetBundle.LoadFromFile(assetInBundlePath);
            assetInBundleConfig = assetInBundleAB.LoadAsset<AssetInBundleConfig>(AssetInBundleConfig.CONFIG_PATH);
            //assetInBundleAB.Unload(false);
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = assetBundleManifest != null && assetInBundleConfig != null;
            return true;
        }

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            AssetBundleLoaderData abLoaderData = loaderData as AssetBundleLoaderData;
            for (int i =0;i<abLoaderData.assetPaths.Length;++i)
            {
                string assetPath = abLoaderData.assetPaths[i];
                if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
                {
                    assetNode.RetainLoadCount();
                    continue;
                }

                string mainBunldePath = assetInBundleConfig.GetBundlePathByPath(assetPath);
                if(bundleNodeDic.TryGetValue(mainBunldePath,out BundleNode bundleNode))
                {
                    bundleNode.RetainRefCount();

                    assetNode = assetNodePool.Get();
                    assetNode.InitNode(assetPath, bundleNode);
                    assetNode.RetainLoadCount();

                    assetNodeDic.Add(assetPath,assetNode);
                    continue;
                }

                LoadAssetBundle(abLoaderData,assetPath,mainBunldePath);
            }
        }

        private void LoadAssetBundle(AssetBundleLoaderData loaderData,string assetPath ,string bundlePath)
        {
            AssetBundleAsyncOperation mainAsyncOperation = (AssetBundleAsyncOperation)asyncOperationORM.GetDataByKey(bundlePath);
            if (mainAsyncOperation != null)
            {
                mainAsyncOperation.RetainRefCount();
            }
            else
            {
                mainAsyncOperation = new AssetBundleAsyncOperation(bundlePath, GetAssetRootPath());
                mainAsyncOperation.RetainRefCount();

                asyncOperationORM.PushData(mainAsyncOperation);
            }
            loaderData.AddAsyncOperation(assetPath, mainAsyncOperation);

            string[] dependBundles = assetBundleManifest.GetAllDependencies(bundlePath);
            if (dependBundles != null && dependBundles.Length > 0)
            {
                Array.ForEach(dependBundles, (path) =>
                {
                    if (bundleNodeDic.TryGetValue(path, out BundleNode bundleNode))
                    {
                        bundleNode.RetainRefCount();
                    }
                    else
                    {
                        if (asyncOperationORM.GetDataByKey(path) is AssetBundleAsyncOperation dependOperation)
                        {
                            dependOperation.RetainRefCount();
                        }
                        else
                        {
                            dependOperation = new AssetBundleAsyncOperation(path, GetAssetRootPath());
                            dependOperation.RetainRefCount();

                            asyncOperationORM.PushData(dependOperation);
                        }

                        loaderData.AddAsyncOperation(assetPath, dependOperation);
                    }
                });
            }
        }
        
        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            AssetBundleLoaderData abLoaderData = loaderData as AssetBundleLoaderData;
            UpdateLoaderDataAsyncOperation(abLoaderData);
            return CheckLoaderData(abLoaderData, loaderHandle);
        }

        private void UpdateLoaderDataAsyncOperation(AssetBundleLoaderData loaderData)
        {
            for (int i = 0; i < loaderData.assetPaths.Length; ++i)
            {
                string assetPath = loaderData.assetPaths[i];
                
                if (loaderData.IsAssetInAsyncOperation(assetPath))
                {
                    if(loaderData.IsAsyncOperationComplete(assetPath))
                    {
                        AssetBundleAsyncOperation[] assetAsyncOperations = loaderData.GetAllOperation(assetPath);
                        foreach(AssetBundleAsyncOperation operation in assetAsyncOperations)
                        {
                            if(!bundleNodeDic.ContainsKey(operation.AssetPath))
                            {
                                BundleNode bundleNode = bundleNodePool.Get();
                                bundleNode.InitNode(operation.AssetPath, operation.GetAsset() as AssetBundle);
                                bundleNode.RefCount = operation.RefCount;

                                bundleNodeDic.Add(operation.AssetPath, bundleNode);
                            }
                        }

                        string bundlePath = assetInBundleConfig.GetBundlePathByPath(assetPath);
                        BundleNode mainBundleNode = bundleNodeDic[bundlePath];

                        AssetNode assetNode = assetNodePool.Get();
                        assetNode.InitNode(assetPath, mainBundleNode);
                        assetNode.RetainLoadCount();

                        assetNodeDic.Add(assetPath, assetNode);

                    }//IsAsyncOperationComplete
                }
            }
        }
        private bool CheckLoaderData(AssetBundleLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            bool isAllAssetLoadComplete = true;
            for (int i = 0; i < loaderData.assetPaths.Length; ++i)
            {
                if(CheckAssetLoadComplete(loaderData,i,loaderHandle))
                {
                    continue;
                }else
                {
                    if(isAllAssetLoadComplete)
                    {
                        isAllAssetLoadComplete = false;
                    }
                    CheckAssetLoadProgress(loaderData, i, loaderHandle);
                }
            }

            loaderData.InvokeBatchProgress(loaderHandle.Progresses());

            if(isAllAssetLoadComplete)
            {
                loaderData.InvokeBatchComplete(loaderHandle.GetObjects());
            }

            return isAllAssetLoadComplete;
        }

        private bool CheckAssetLoadComplete(AssetBundleLoaderData loaderData, int index,AssetLoaderHandle loaderHandle)
        {
            string assetPath = loaderData.assetPaths[index];
            if (assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
            {
                if (!loaderData.GetIsCompleteCalled(index))
                {
                    assetNode.ReleaseLoadCount();

                    UnityObject assetObj = null;
                    if (loaderData.isInstance)
                    {
                        assetObj = assetNode.GetInstance();
                    }
                    else
                    {
                        assetObj = assetNode.GetAsset();
                    }

                    loaderHandle.SetObject(index, assetObj);
                    loaderHandle.SetProgress(index, 1.0f);

                    loaderData.InvokeProgress(index, 1.0f);
                    loaderData.InvokeComplete(index, assetObj);
                }

                return true;
            }

            return false;
        }
        private void CheckAssetLoadProgress(AssetBundleLoaderData loaderData, int index, AssetLoaderHandle loaderHandle)
        {
            string assetPath = loaderData.assetPaths[index];
            string bundlePath = assetInBundleConfig.GetBundlePathByPath(assetPath);
            string[] depends = assetBundleManifest.GetAllDependencies(bundlePath);

            int bundleCount = 1;
            if (depends != null && depends.Length > 0)
            {
                bundleCount += depends.Length;
            }
            AssetBundleAsyncOperation[] operations = loaderData.GetAllOperation(assetPath);
            float totalProgress = 0.0f;
            foreach (var operation in operations)
            {
                totalProgress += operation.Progress();
            }
            if (operations != null && operations.Length != bundleCount)
            {
                totalProgress += (bundleCount - operations.Length);
            }

            float progress = loaderHandle.GetProgress(index);
            float newProgress = totalProgress / bundleCount;
            if(progress!=newProgress)
            {
                loaderHandle.SetProgress(index, newProgress);
                loaderData.InvokeProgress(index, newProgress);
            }
        }

        protected override string GetAssetRootPath()
        {
            return assetBundleRootPath + "/";
        }
    }
}
