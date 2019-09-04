using Dot.Core.Loader.Config;
using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        private readonly ObjectPool<AssetBundleLoaderData> loaderDataPool = new ObjectPool<AssetBundleLoaderData>(4);
        private Dictionary<string, AssetNode> assetNodeDic = new Dictionary<string, AssetNode>();
        private Dictionary<string, BundleNode> bundleNodeDic = new Dictionary<string, BundleNode>();

        protected override AssetLoaderData GetLoaderData() => loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetBundleLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            AssetBundleLoaderData abLoaderData = loaderData as AssetBundleLoaderData;
            AssetLoaderHandle handle = loaderHandleDic[abLoaderData.uniqueID];

            for (int i =0;i<abLoaderData.assetPaths.Length;++i)
            {
                string assetPath = abLoaderData.assetPaths[i];
                if(assetNodeDic.TryGetValue(assetPath,out AssetNode assetNode))
                {
                    if(assetNode.IsAlive())
                    {

                    }
                }
            }
        }
        
        public override void Initialize(Action<bool> initCallback, params object[] sysObjs)
        {
            base.Initialize(initCallback, sysObjs);
            assetBundleRootPath = sysObjs[0] as string;

            string manifestPath = $"{assetBundleRootPath}/{AssetBundleConst.ASSETBUNDLE_MAINFEST_NAME}";
            AssetBundle manifestAB = AssetBundle.LoadFromFile(manifestPath);
            assetBundleManifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifestAB.Unload(false);
            
            string assetInBundlePath = $"{assetBundleRootPath}/{AssetInBundleConfig.CONFIG_ASSET_BUNDLE_NAME}";
            AssetBundle assetInBundleAB = AssetBundle.LoadFromFile(assetInBundlePath);
            assetInBundleConfig = assetInBundleAB.LoadAsset<AssetInBundleConfig>(AssetInBundleConfig.CONFIG_PATH);
            assetInBundleAB.Unload(false);
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = assetBundleManifest != null && assetInBundleConfig != null;
            return true;
        }

        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            return true;
        }

        private string[] GetAssetBundleDepend(string assetPath)
        {
            string bundlePath = assetInBundleConfig.GetBundlePathByPath(assetPath);
            return assetBundleManifest.GetAllDependencies(bundlePath);
        }
    }
}
