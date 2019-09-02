using Dot.Core.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.Loader
{
    public static class AssetBundleConst
    {

    }

    public class AssetBundleLoader : AAssetLoader
    {
        private readonly ObjectPool<AssetBundleLoaderData> loaderDataPool = new ObjectPool<AssetBundleLoaderData>(4);
        private string assetBundleRootPath = "";

        protected override AssetLoaderData GetLoaderData() => loaderDataPool.Get();

        protected override void ReleaseLoaderData(AssetLoaderData loaderData) => loaderDataPool.Release(loaderData as AssetBundleLoaderData);

        protected override void StartLoaderDataLoading(AssetLoaderData loaderData)
        {
            
        }
        
        public override void Initialize(Action<bool> initCallback, params object[] sysObjs)
        {
            base.Initialize(initCallback, sysObjs);
            assetBundleRootPath = sysObjs[0] as string;
        }

        protected override bool UpdateInitialize(out bool isSuccess)
        {
            isSuccess = true;
            return true;
        }

        protected override bool UpdateLoadingLoaderData(AssetLoaderData loaderData, AssetLoaderHandle loaderHandle)
        {
            return true;
        }
    }
}
