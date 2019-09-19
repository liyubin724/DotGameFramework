using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using Dot.Core.Timer;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ObjectPool<AssetNode> assetNodePool = new ObjectPool<AssetNode>(50);
        private readonly ObjectPool<BundleNode> bundleNodePool = new ObjectPool<BundleNode>(50);

        private Dictionary<string, AssetNode> assetNodeDic = new Dictionary<string, AssetNode>();
        private Dictionary<string, BundleNode> bundleNodeDic = new Dictionary<string, BundleNode>();

        private float assetCleanInterval = 300;
        private TimerTaskInfo assetCleanTimer = null;
        
        private string assetRootDir = "";
        private AssetBundleManifest assetBundleManifest = null;
        protected override void InnerInitialize(string rootDir)
        {
            assetRootDir = rootDir;
            if(!string.IsNullOrEmpty(assetRootDir) && !assetRootDir.EndsWith("/"))
            {
                assetRootDir += "/";
            }

           assetCleanTimer = TimerManager.GetInstance().AddIntervalTimer(assetCleanInterval, this.OnCleanAssetInterval);

            string manifestPath = $"{this.assetRootDir}/{AssetBundleConst.ASSETBUNDLE_MAINFEST_NAME}";
            AssetBundle manifestAB = AssetBundle.LoadFromFile(manifestPath);
            assetBundleManifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifestAB.Unload(false);

            string assetAddressConfigPath = $"{this.assetRootDir}/{AssetAddressConfig.CONFIG_ASSET_BUNDLE_NAME}";
            AssetBundle assteAddressConfigAB = AssetBundle.LoadFromFile(assetAddressConfigPath);
            assetAddressConfig = assteAddressConfigAB.LoadAsset<AssetAddressConfig>(AssetAddressConfig.CONFIG_PATH);
            assteAddressConfigAB.Unload(false);
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = true;
            if(assetBundleManifest == null)
            {
                isSuccess = false;
            }
            if(isSuccess && pathMode == AssetPathMode.Address && assetAddressConfig == null)
            {
                isSuccess = false;
            }

            return true;
        }
        
        private Dictionary<string, AssetBundleAsyncOperation> loadingAsyncOperationDic = new Dictionary<string, AssetBundleAsyncOperation>();
        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            for (int i = 0; i < loaderData.assetPaths.Length; ++i)
            {
                string assetPath = loaderData.assetPaths[i];
                if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
                {
                    assetNode.RetainLoadCount();
                    continue;
                }

                string mainBundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
                if(!bundleNodeDic.TryGetValue(mainBundlePath,out BundleNode bundleNode))
                {
                    bundleNode = CreateBundleNode(mainBundlePath);
                }
                assetNode = assetNodePool.Get();
                assetNode.InitNode(assetPath, bundleNode);
                assetNode.RetainLoadCount();

                assetNodeDic.Add(assetPath, assetNode);
            }
        }

        private BundleNode CreateBundleNode(string mainBundlePath)
        {
            if(!bundleNodeDic.TryGetValue(mainBundlePath,out BundleNode mainBundleNode))
            {
                CreateAsyncOperaton(mainBundlePath);

                mainBundleNode = bundleNodePool.Get();
                mainBundleNode.InitNode(mainBundlePath);

                bundleNodeDic.Add(mainBundlePath, mainBundleNode);
            }

            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            if (dependBundlePaths != null && dependBundlePaths.Length > 0)
            {
                foreach (var path in dependBundlePaths)
                {
                    if (!bundleNodeDic.TryGetValue(path, out BundleNode dependBundleNode))
                    {
                        dependBundleNode = CreateBundleNode(path);
                    }
                    mainBundleNode.AddDependNode(dependBundleNode);                   
                }
            }

            return mainBundleNode;
        }

        private void CreateAsyncOperaton(string bundlePath)
        {
            AssetBundleAsyncOperation operation = new AssetBundleAsyncOperation(bundlePath, assetRootDir);

            loadingAsyncOperationList.Add(operation);
            loadingAsyncOperationDic.Add(bundlePath,operation);
        }
        
        protected override void OnAsyncOperationLoaded(AAssetAsyncOperation operation)
        {
            BundleNode bundleNode = bundleNodeDic[operation.AssetPath];
            bundleNode.SetAssetBundle((operation.GetAsset() as AssetBundle));

            loadingAsyncOperationDic.Remove(operation.AssetPath);
        }
        

        private float GetAssetLoadingProgress(string assetPath)
        {
            float progress = 0.0f;
            int totalCount = 0;
            string mainBundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
            if (loadingAsyncOperationDic.TryGetValue(mainBundlePath,out AssetBundleAsyncOperation mainOperation))
            {
                progress += mainOperation.Progress();
            }else
            {
                progress += 1.0f;
            }
            ++totalCount;
            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            if(dependBundlePaths!=null && dependBundlePaths.Length>0)
            {
                foreach(var path in dependBundlePaths)
                {
                    if (loadingAsyncOperationDic.TryGetValue(path, out AssetBundleAsyncOperation operation))
                    {
                        progress += operation.Progress();
                    }
                    else
                    {
                        progress += 1.0f;
                    }
                }
                totalCount += dependBundlePaths.Length;
            }
            return progress / totalCount;
        }

        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData)
        {
            AssetLoaderHandle loaderHandle = null;
            if (loaderHandleDic.ContainsKey(loaderData.uniqueID))
            {
                loaderHandle = loaderHandleDic[loaderData.uniqueID];
            }

            bool isComplete = true;
            for (int i = 0; i < loaderData.assetPaths.Length; ++i)
            {
                if(loaderData.GetLoadState(i))
                {
                    continue;
                }
                string assetPath = loaderData.assetPaths[i];
                AssetNode assetNode = assetNodeDic[assetPath];
                if(assetNode.IsDone)
                {
                    assetNode.ReleaseLoadCount();
                    UnityObject uObj = assetNode.GetAsset();
                    if (uObj == null)
                    {
                        Debug.LogError($"AssetBundleLoader::AssetLoadComplete->asset is null.path = {assetPath}");
                    }

                    if (uObj != null && loaderData.isInstance)
                    {
                        uObj = UnityObject.Instantiate(uObj);
                    }

                    loaderHandle.SetObject(i, uObj);
                    loaderHandle.SetProgress(i, 1.0f);

                    loaderData.InvokeComplete(i, uObj);
                }
                else
                {
                    float progress = GetAssetLoadingProgress(assetPath);
                    float oldProgress = loaderHandle.GetProgress(i);
                    if (oldProgress != progress)
                    {
                        loaderHandle.SetProgress(i, progress);
                        loaderData.InvokeProgress(i, progress);
                    }
                    isComplete = false;
                }
            }

            loaderData.InvokeBatchProgress(loaderHandle.AssetProgresses);
            if (isComplete)
            {
                loaderHandle.isDone = true;
                loaderData.InvokeBatchComplete(loaderHandle.AssetObjects);
            }

            return isComplete;
        }
        
        private void OnCleanAssetInterval(System.Object userData)
        {
            string[] assetNodeKeys = (from nodeKVP in assetNodeDic where !nodeKVP.Value.IsAlive() select nodeKVP.Key).ToArray();
            foreach (var key in assetNodeKeys)
            {
                AssetNode assetNode = assetNodeDic[key];
                assetNodeDic.Remove(key);
                assetNodePool.Release(assetNode);
            }

            string[] bundleNodeKeys = (from nodeKVP in bundleNodeDic where nodeKVP.Value.RefCount == 0 select nodeKVP.Key).ToArray();
            foreach(var key in bundleNodeKeys)
            {
                BundleNode bundleNode = bundleNodeDic[key];
                bundleNodeDic.Remove(key);
                bundleNodePool.Release(bundleNode);
            }
        }

        public override void UnloadAsset(string pathOrAddress)
        {
            string assetPath = GetAssetPath(pathOrAddress);
            if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
            {
                assetNodeDic.Remove(assetPath);
                assetNodePool.Release(assetNode);

                OnCleanAssetInterval(null);
            }
        }

        protected override void UnloadLoadingAssetLoader(AssetLoaderData loaderData)
        {
        }

        protected override void InnerUnloadUnusedAssets()
        {
            OnCleanAssetInterval(null);
        }

        public override UnityObject InstantiateAsset(string assetPath, UnityObject asset)
        {
            if(pathMode == AssetPathMode.Address)
            {
                assetPath = assetAddressConfig.GetAssetPathByAddress(assetPath);
            }
            if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
            {
                if(assetNode.IsDone)
                {
                    UnityObject instance = base.InstantiateAsset(assetPath, asset);
                    assetNode.AddInstance(instance);
                    return instance;
                }
            }
            return null;
        }
    }
}
