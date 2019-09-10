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
        private readonly ObjectPool<AssetNode> assetNodePool = new ObjectPool<AssetNode>(50);
        private readonly ObjectPool<BundleNode> bundleNodePool = new ObjectPool<BundleNode>(50);

        private Dictionary<string, AssetNode> assetNodeDic = new Dictionary<string, AssetNode>();
        private Dictionary<string, BundleNode> bundleNodeDic = new Dictionary<string, BundleNode>();

        private float assetCleanInterval = 300;
        private TimerTaskInfo assetCleanTimer = null;
        
        private string assetRootDir = "";
        private AssetBundleManifest assetBundleManifest = null;
        private AssetAddressConfig assetAddressConfig = null;
        private AssetPathMode pathMode = AssetPathMode.Address;
        protected override void InnerInitialize(AssetPathMode pathMode, string rootDir)
        {
            this.pathMode = pathMode;
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

        private readonly ObjectPool<AssetBundleLoaderData> loaderDataPool = new ObjectPool<AssetBundleLoaderData>(5);
        protected override AssetLoaderData GetLoaderData(string[] assetPaths)
        {
            AssetBundleLoaderData loaderData = loaderDataPool.Get();

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
        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetBundleLoaderData);

        private Dictionary<string, AssetBundleAsyncOperation> loadingAsyncOperationDic = new Dictionary<string, AssetBundleAsyncOperation>();
        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            AssetBundleLoaderData abLoaderData = loaderData as AssetBundleLoaderData;
            for (int i = 0; i < abLoaderData.assetPaths.Length; ++i)
            {
                string assetPath = abLoaderData.assetPaths[i];
                if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
                {
                    assetNode.RetainLoadCount();
                    continue;
                }
                
                string mainBundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);
                if(IsBundleNodeLoaded(mainBundlePath))
                {
                    assetNode = assetNodePool.Get();
                    assetNode.InitNode(assetPath, bundleNodeDic[mainBundlePath]);
                    assetNode.RetainLoadCount();

                    assetNodeDic.Add(assetPath,assetNode);
                    continue;
                }

                if(IsBundleLoading(mainBundlePath))
                {
                    continue;
                }

                LoadAssetBundle(mainBundlePath);
            }
        }

        private void LoadAssetBundle(string mainBundlePath)
        {
            if(bundleNodeDic.ContainsKey(mainBundlePath) && !loadingAsyncOperationDic.ContainsKey(mainBundlePath))
            {
                CreateAsyncOperaton(mainBundlePath);
            }
            string[] dependBundlePaths = assetBundleManifest.GetAllDependencies(mainBundlePath);
            if (dependBundlePaths != null && dependBundlePaths.Length > 0)
            {
                foreach (var path in dependBundlePaths)
                {
                    if (bundleNodeDic.ContainsKey(path) && !loadingAsyncOperationDic.ContainsKey(path))
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

        private bool IsBundleLoading(string mainBundlePath)
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

        private bool IsBundleNodeLoaded(string mainBundlePath)
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
        
        
        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            AssetBundleLoaderData abLoaderData = loaderData as AssetBundleLoaderData;
            for (int i = 0; i < abLoaderData.assetPaths.Length; ++i)
            {
                string assetPath = abLoaderData.assetPaths[i];
                string mainBundlePath = assetAddressConfig.GetBundlePathByPath(assetPath);

            }
            return true;
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

        protected override void UnloadLoadingAssetLoader(AssetLoaderData loaderData, AssetLoaderHandle handle, bool destroyIfLoaded)
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
