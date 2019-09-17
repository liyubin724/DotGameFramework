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
                if(IsInBundleNode(mainBundlePath))
                {
                    assetNode = assetNodePool.Get();
                    assetNode.InitNode(assetPath, bundleNodeDic[mainBundlePath]);
                    assetNode.RetainLoadCount();

                    assetNodeDic.Add(assetPath,assetNode);
                    continue;
                }

                if(IsInBundleLoading(mainBundlePath))
                {
                    continue;
                }

                LoadAssetBundle(mainBundlePath);
            }
        }

        private void LoadAssetBundle(string mainBundlePath)
        {
            if(!bundleNodeDic.ContainsKey(mainBundlePath) && !loadingAsyncOperationDic.ContainsKey(mainBundlePath))
            {
                CreateAsyncOperaton(mainBundlePath);
            }
            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            if (dependBundlePaths != null && dependBundlePaths.Length > 0)
            {
                foreach (var path in dependBundlePaths)
                {
                    if (!bundleNodeDic.ContainsKey(path) && !loadingAsyncOperationDic.ContainsKey(path))
                    {
                        CreateAsyncOperaton(path);
                    }
                }
            }
        }

        private void CreateAsyncOperaton(string bundlePath)
        {
            AssetBundleAsyncOperation operation = new AssetBundleAsyncOperation(bundlePath, assetRootDir);
            loadingAsyncOperationList.Add(operation);
            loadingAsyncOperationDic.Add(bundlePath,operation);
        }

        private bool IsInBundleLoading(string mainBundlePath)
        {
            if(loadingAsyncOperationDic.ContainsKey(mainBundlePath))
            {
                return true;
            }
            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            if(dependBundlePaths!=null && dependBundlePaths.Length>0)
            {
                foreach(var path in dependBundlePaths)
                {
                    if(loadingAsyncOperationDic.ContainsKey(path))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsInBundleNode(string mainBundlePath)
        {
            if(!bundleNodeDic.ContainsKey(mainBundlePath))
            {
                return false;
            }
            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            if (dependBundlePaths != null && dependBundlePaths.Length > 0)
            {
                foreach (var path in dependBundlePaths)
                {
                    if (!bundleNodeDic.ContainsKey(path))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected override void OnAsyncOperationLoaded()
        {
            List<string> assetPaths = (from loaderData in loaderDataLoadingList
                                       from assetPath in loaderData.assetPaths
                                       select assetPath).ToList();

            Dictionary<string, int> assetPathDic = new Dictionary<string, int>();
            assetPaths.ForEach((assetPath) =>
            {
                if(assetPathDic.ContainsKey(assetPath))
                {
                    assetPathDic[assetPath]++;
                }else
                {
                    assetPathDic.Add(assetPath, 1);
                }
            });

            foreach(var kvp in assetPathDic)
            {
                if(!assetNodeDic.ContainsKey(kvp.Key))
                {
                    if(IsAssetLoaded(kvp.Key))
                    {
                        AssetNode assetNode = CreateAssetNodeByOperation(kvp.Key);
                        assetNode.LoadCount = kvp.Value;
                    }
                }
            }
        }

        private AssetNode CreateAssetNodeByOperation(string assetPath)
        {
            AssetNode assetNode = assetNodePool.Get();
            string mainBundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
            if(loadingAsyncOperationDic.TryGetValue(mainBundlePath,out AssetBundleAsyncOperation mainOperation))
            {
                BundleNode bn = bundleNodePool.Get();
                bn.InitNode(mainBundlePath, mainOperation.GetAsset() as AssetBundle);
                bundleNodeDic.Add(mainBundlePath, bn);

                loadingAsyncOperationDic.Remove(mainBundlePath);
            }
            BundleNode mainBundleNode = bundleNodeDic[mainBundlePath];
            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            foreach(var path in dependBundlePaths)
            {
                if(loadingAsyncOperationDic.TryGetValue(path,out AssetBundleAsyncOperation operation))
                {
                    BundleNode bn = bundleNodePool.Get();
                    bn.InitNode(path, operation.GetAsset() as AssetBundle);
                    bundleNodeDic.Add(path, bn);

                    loadingAsyncOperationDic.Remove(path);
                }

                BundleNode dependNode = bundleNodeDic[path];
                dependNode.RetainRefCount();
            }

            assetNode.InitNode(assetPath, mainBundleNode);

            assetNodeDic.Add(assetPath, assetNode);
            return assetNode;
        }

        private bool IsAssetLoaded(string assetPath)
        {
            string mainBundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
            if (!IsBundleLoaded(mainBundlePath))
            {
                return false;
            }
            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            if(dependBundlePaths!=null && dependBundlePaths.Length>0)
            {
                foreach(var path in dependBundlePaths)
                {
                    if(!IsBundleLoaded(path))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsBundleLoaded(string bundlePath)
        {
            if(bundleNodeDic.ContainsKey(bundlePath))
            {
                return true;
            }
            if (loadingAsyncOperationDic.TryGetValue(bundlePath, out AssetBundleAsyncOperation operation))
            {
                if(operation.Status == AssetAsyncOperationStatus.Loaded)
                {
                    return true;
                }
            }
            return false;
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
            if(loaderHandle == null)
            {
                loaderData.BreakLoader();
            }

            bool isComplete = true;
            for (int i = 0; i < loaderData.assetPaths.Length; ++i)
            {
                if(loaderData.GetLoadState(i))
                {
                    continue;
                }
                string assetPath = loaderData.assetPaths[i];

                if(loaderHandle == null)
                {
                    if (assetNodeDic.TryGetValue(assetPath, out AssetNode node))
                    {
                        node.ReleaseLoadCount();
                    }else
                    {
                        isComplete = false;
                    }
                    continue;
                }

                if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
                {
                    assetNode.ReleaseLoadCount();

                    UnityObject uObj = assetNode.GetAsset();
                    if (uObj == null)
                    {
                        Debug.LogError($"AssetBundleLoader::UpdateLoadingLoaderData->asset is null.path = {assetPath}");
                    }

                    if (uObj != null && loaderData.isInstance)
                    {
                        uObj = UnityObject.Instantiate(uObj);
                    }
                    loaderHandle.SetObject(i, uObj);
                    loaderHandle.SetProgress(i, 1.0f);

                    loaderData.InvokeComplete(i, uObj);
                    continue;
                }

                float oldProgress = loaderHandle.GetProgress(i);
                float curProgress = GetAssetLoadingProgress(assetPath);
                if (oldProgress != curProgress)
                {
                    loaderHandle.SetProgress(i, curProgress);

                    loaderData.InvokeProgress(i, curProgress);
                }
                isComplete = false;
            }

            if(loaderHandle!=null)
            {
                loaderData.InvokeBatchProgress(loaderHandle.AssetProgresses);
                if (isComplete)
                {
                    loaderHandle.isDone = true;
                    loaderData.InvokeBatchComplete(loaderHandle.AssetObjects);
                }
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

                string mainBundlePath = assetAddressConfig.GetBundlePathByPath(key);
                BundleNode bundleNode = bundleNodeDic[mainBundlePath];
                if (bundleNode.RefCount == 0)
                {
                    string[] depends = assetBundleManifest.GetAllDependencies(mainBundlePath);
                    foreach (var path in depends)
                    {
                        bundleNodeDic[path].ReleaseRefCount();
                    }
                }
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

                string mainBundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
                BundleNode bundleNode = bundleNodeDic[mainBundlePath];
                List<string> unloadBundleNodes = null;
                if (bundleNode.RefCount == 0)
                {
                    unloadBundleNodes = new List<string>();
                    unloadBundleNodes.Add(mainBundlePath);

                    string[] depends = assetBundleManifest.GetAllDependencies(mainBundlePath);
                    foreach (var path in depends)
                    {
                        bundleNodeDic[path].ReleaseRefCount();
                        if(bundleNodeDic[path].RefCount == 0)
                        {
                            unloadBundleNodes.Add(path);
                        }
                    }
                }

                if(unloadBundleNodes!=null && unloadBundleNodes.Count>0)
                {
                    foreach (var key in unloadBundleNodes)
                    {
                        BundleNode node = bundleNodeDic[key];
                        bundleNodeDic.Remove(key);
                        bundleNodePool.Release(node);
                    }
                }

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
                UnityObject instance = base.InstantiateAsset(assetPath, asset);
                assetNode.AddInstance(instance);
                return instance;
            }
            return null;
        }
    }
}
