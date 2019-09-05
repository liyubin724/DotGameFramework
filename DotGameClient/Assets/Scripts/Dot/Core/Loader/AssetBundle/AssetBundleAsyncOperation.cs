using UnityEngine;

namespace Dot.Core.Loader
{
    public class AssetBundleAsyncOperation : AAssetAsyncOperation
    {
        private AssetBundleCreateRequest asyncOperation = null;

        public AssetBundleAsyncOperation(string assetPath, string assetRoot) : base(assetPath, assetRoot)
        {
        }

        public override void DoUpdate()
        {
            if (status == AssetAsyncOperationStatus.Loading)
            {
                if (asyncOperation.isDone)
                {
                    status = AssetAsyncOperationStatus.Loaded;
                }
            }
        }

        public override UnityEngine.Object GetAsset()
        {
            if (status == AssetAsyncOperationStatus.Loaded)
            {
                return asyncOperation.assetBundle;
            }
            return null;
        }

        public override float Progress()
        {
            if (status == AssetAsyncOperationStatus.Loaded)
            {
                return 1;
            }
            else if (status == AssetAsyncOperationStatus.Loading)
            {
                return asyncOperation.progress;
            }
            else
            {
                return 0;
            }
        }

        protected override void CreateAsyncOperation()
        {
            asyncOperation = AssetBundle.LoadFromFileAsync(assetRootPath+assetPath);
        }
    }
}
