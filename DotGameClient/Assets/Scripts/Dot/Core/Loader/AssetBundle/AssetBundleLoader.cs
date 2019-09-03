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
        public static readonly string ASSET_DETAIL_NAME = "assetdetail.txt";
    }

    public class AssetBundleLoader : AAssetLoader
    {
        private string assetBundleRootPath = "";
        private AssetBundleManifest assetBundleManifest = null;
        private AssetBundleCreateRequest manifestRequest = null;
        //private AssetBundleDetailData assetBundleDetailData = null;

        private readonly ObjectPool<AssetBundleLoaderData> loaderDataPool = new ObjectPool<AssetBundleLoaderData>(4);

        protected override AssetLoaderData GetLoaderData() => loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetBundleLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            
        }
        
        public override void Initialize(Action<bool> initCallback, params object[] sysObjs)
        {
            base.Initialize(initCallback, sysObjs);
            assetBundleRootPath = sysObjs[0] as string;
            string manifestPath = Path.Combine(assetBundleRootPath, AssetBundleConst.ASSETBUNDLE_MAINFEST_NAME);
            manifestRequest = AssetBundle.LoadFromFileAsync(manifestPath);
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = false;
            if(manifestRequest!=null && manifestRequest.isDone)
            {
                if(manifestRequest.assetBundle!=null)
                {
                    assetBundleManifest = manifestRequest.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    if(assetBundleManifest!=null)
                    {
                        isSuccess = true;
                    }
                }
                return true;
            }
            return false;
        }

        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            return true;
        }
    }
}
