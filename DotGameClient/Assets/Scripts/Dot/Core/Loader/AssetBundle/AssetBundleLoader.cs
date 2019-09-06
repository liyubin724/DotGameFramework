using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using Dot.Core.Timer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using SystemObject = System.Object;
using Dot.Core.Generic;

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

        private readonly ObjectPool<AssetBundleLoaderData> loaderDataPool = new ObjectPool<AssetBundleLoaderData>(5);
        private readonly ObjectPool<AssetNode> assetNodePool = new ObjectPool<AssetNode>(50);
        private readonly ObjectPool<BundleNode> bundleNodePool = new ObjectPool<BundleNode>(50);

        private Dictionary<string, AssetNode> assetNodeDic = new Dictionary<string, AssetNode>();
        private Dictionary<string, BundleNode> bundleNodeDic = new Dictionary<string, BundleNode>();

        private float assetCleanInterval = 300;
        private TimerTaskInfo assetCleanTimer = null;

        protected override AssetLoaderData GetLoaderData() => loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetBundleLoaderData);

        public override void Initialize(AssetPathMode pathMode, Action<bool> initCallback, params SystemObject[] sysObjs)
        {
            base.Initialize(pathMode,initCallback, sysObjs);
            assetBundleRootPath = sysObjs[0] as string;

            if(sysObjs.Length>1)
            {
                assetCleanInterval = (float)sysObjs[1];
                if(assetCleanInterval<=0)
                {
                    assetCleanInterval = 300;
                }
            }

            assetCleanTimer = TimerManager.GetInstance().AddIntervalTimer(assetCleanInterval, OnCleanAssetInterval);

            string manifestPath = $"{assetBundleRootPath}/{AssetBundleConst.ASSETBUNDLE_MAINFEST_NAME}";
            AssetBundle manifestAB = AssetBundle.LoadFromFile(manifestPath);
            assetBundleManifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifestAB.Unload(false);

            string assetAddressConfigPath = $"{assetBundleRootPath}/{AssetAddressConfig.CONFIG_ASSET_BUNDLE_NAME}";
            AssetBundle assteAddressConfigAB = AssetBundle.LoadFromFile(assetAddressConfigPath);
            assetAddressConfig = assteAddressConfigAB.LoadAsset<AssetAddressConfig>(AssetAddressConfig.CONFIG_PATH);
            assteAddressConfigAB.Unload(false);
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = assetBundleManifest != null && assetAddressConfig != null;
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

                string mainBunldePath = assetAddressConfig.GetBundlePathByPath(assetPath);
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
            AssetBundleAsyncOperation mainAsyncOperation = (AssetBundleAsyncOperation)asyncOperations.GetDataByKey(bundlePath);
            if (mainAsyncOperation != null)
            {
                mainAsyncOperation.RetainRefCount();
            }
            else
            {
                mainAsyncOperation = new AssetBundleAsyncOperation(bundlePath, GetAssetRootPath());
                mainAsyncOperation.RetainRefCount();

                asyncOperations.PushData(mainAsyncOperation);
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
                        if (asyncOperations.GetDataByKey(path) is AssetBundleAsyncOperation dependOperation)
                        {
                            dependOperation.RetainRefCount();
                        }
                        else
                        {
                            dependOperation = new AssetBundleAsyncOperation(path, GetAssetRootPath());
                            dependOperation.RetainRefCount();

                            asyncOperations.PushData(dependOperation);
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
                    if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
                    {
                        assetNode.RetainLoadCount();
                        loaderData.DeleteAssetAsyncOperation(assetPath);
                    }else
                    {
                        if (loaderData.IsAsyncOperationComplete(assetPath))
                        {
                            AssetBundleAsyncOperation[] assetAsyncOperations = loaderData.GetAllOperation(assetPath);
                            foreach (AssetBundleAsyncOperation operation in assetAsyncOperations)
                            {
                                if (!bundleNodeDic.ContainsKey(operation.AssetPath))
                                {
                                    BundleNode bundleNode = bundleNodePool.Get();
                                    bundleNode.InitNode(operation.AssetPath, operation.GetAsset() as AssetBundle);
                                    bundleNode.RefCount = operation.RefCount;

                                    bundleNodeDic.Add(operation.AssetPath, bundleNode);
                                }
                            }

                            string bundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
                            BundleNode mainBundleNode = bundleNodeDic[bundlePath];

                            assetNode = assetNodePool.Get();
                            assetNode.InitNode(assetPath, mainBundleNode);
                            assetNode.RetainLoadCount();

                            assetNodeDic.Add(assetPath, assetNode);
                            loaderData.DeleteAssetAsyncOperation(assetPath);
                        }//IsAsyncOperationComplete
                    }
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
            string bundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
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

        private List<string> assetDicKeys = new List<string>();
        private void OnCleanAssetInterval(System.Object userData)
        {
            assetDicKeys.AddRange(assetNodeDic.Keys);
            foreach (string key in assetDicKeys)
            {
                AssetNode assetNode = assetNodeDic[key];
                if(!assetNode.IsAlive())
                {
                    assetNodeDic.Remove(key);
                    assetNodePool.Release(assetNode);
                }
            }

            assetDicKeys.Clear();
            assetDicKeys.AddRange(bundleNodeDic.Keys);
            foreach(string key in assetDicKeys)
            {
                BundleNode bundleNode = bundleNodeDic[key];
                if(bundleNode.RefCount == 0)
                {
                    string[] depends = assetBundleManifest.GetAllDependencies(key);
                    foreach(var path in depends)
                    {
                        bundleNodeDic[path].ReleaseRefCount();
                    }
                    bundleNodeDic.Remove(key);
                    bundleNodePool.Release(bundleNode);
                }
                else if(bundleNode.RefCount<0)
                {
                    Debug.LogError("AssetBundleLoader::OnCleanAssetInterval->bundle node's refcount is less 0.path = " + key);
                }
            }
            assetDicKeys.Clear();
        }

        protected override void InnerUnloadUnusedAssets()
        {
            OnCleanAssetInterval(null);
        }

        protected override string GetAssetRootPath()
        {
            return assetBundleRootPath + "/";
        }

        public override UnityObject InstantiateAsset(string assetPath, UnityObject asset)
        {
            if(pathMode == AssetPathMode.Address)
            {
                assetPath = assetAddressConfig.GetAssetPathByAddress(assetPath);
            }
            if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
            {
                UnityObject instance = base.InstantiateAsset(assetPath, asset);
                assetNode.AddInstance(instance);
                return instance;
            }
            return null;
        }

        protected IndexMapORM<string, AssetBundleAsyncOperation> asyncOperations = new IndexMapORM<string, AssetBundleAsyncOperation>();
        protected override void DeleteAsyncOperation(int index)
        {
            asyncOperations.DeleteByIndex(index);
        }

        protected override AAssetAsyncOperation GetAsyncOperation(int index)
        {
            return asyncOperations.GetDataByIndex(index);
        }

        protected override int GetAsyncOperationCount()
        {
            return asyncOperations.Count;
        }
    }
}
