using UnityEngine;

namespace Dot.Core.Loader
{
    public class ResourceAsyncOperation : AAssetAsyncOperation
    {
        private ResourceRequest asyncOperation = null;

        public ResourceAsyncOperation(string assetPath, string assetRoot) : base(assetPath, assetRoot)
        {
        }

        protected override void CreateAsyncOperation()
        {
            asyncOperation = Resources.LoadAsync(assetPath);
        }

        public override void DoUpdate()
        {
            if(status == AssetAsyncOperationStatus.Loading)
            {
                if(asyncOperation.isDone)
                {
                    status = AssetAsyncOperationStatus.Loaded;
                }
            }
        }

        public override UnityEngine.Object GetAsset()
        {
            if(status == AssetAsyncOperationStatus.Loaded)
            {
                return asyncOperation.asset;
            }
            return null;
        }

        public override float Progress()
        {
            if(status == AssetAsyncOperationStatus.Loaded)
            {
                return 1;
            }else if(status == AssetAsyncOperationStatus.Loading)
            {
                return asyncOperation.progress;
            }else
            {
                return 0;
            }
        }
    }
}
