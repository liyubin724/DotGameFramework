using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public enum AssetAsyncOperationStatus
    {
        None,
        Loading,
        Loaded,
    }

    public abstract class AAssetAsyncOperation
    {
        protected string assetPath;
        protected string assetRootPath;
        protected AssetAsyncOperationStatus status = AssetAsyncOperationStatus.None;
        
        public string AssetPath { get => assetPath; }

        public AAssetAsyncOperation(string assetPath,string assetRoot)
        {
            this.assetPath = assetPath;
            assetRootPath = assetRoot;
        }

        public AssetAsyncOperationStatus Status { get => status; internal set => status = value; }

        public void StartAsync()
        {
            status = AssetAsyncOperationStatus.Loading;
            CreateAsyncOperation();
        }
        
        protected abstract void CreateAsyncOperation();
        public abstract void DoUpdate();
        public abstract UnityObject GetAsset();
        public abstract float Progress();
    }
}
