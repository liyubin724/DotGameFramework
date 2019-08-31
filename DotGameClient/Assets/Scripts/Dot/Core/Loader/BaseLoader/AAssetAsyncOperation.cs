using Dot.Core.Generic;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public enum AssetAsyncOperationStatus
    {
        None,
        Loading,
        Loaded,
    }

    public abstract class AAssetAsyncOperation : IORMData<string>
    {
        protected string assetPath;
        protected AssetAsyncOperationStatus status = AssetAsyncOperationStatus.None;
        protected int loadRefCount = 0;

        public AAssetAsyncOperation(string assetPath)
        {
            this.assetPath = assetPath;
        }

        public AssetAsyncOperationStatus Status { get => status; }

        public void StartAsync()
        {
            status = AssetAsyncOperationStatus.Loading;
            CreateAsyncOperation();
        }

        public bool IsInLoading() => loadRefCount > 0;
        public void RetainRefCount() => ++loadRefCount;
        public void ReleaseRefCount() => --loadRefCount;

        public string GetKey() => assetPath;

        public abstract void CreateAsyncOperation();
        public abstract void DoUpdate();
        public abstract UnityObject GetAsset();
        public abstract float Progress();

    }
}
