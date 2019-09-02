using Dot.Core.Generic;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public enum AssetAsyncOperationStatus
    {
        None,
        Loading,
        Loaded,
        Error,
    }

    public abstract class AAssetAsyncOperation
    {
        protected string assetPath;
        protected AssetAsyncOperationStatus status = AssetAsyncOperationStatus.None;
        protected int loadRefCount = 0;

        public AAssetAsyncOperation(string assetPath)
        {
            this.assetPath = assetPath;
        }

        public AssetAsyncOperationStatus Status { get => status; internal set => status = value; }

        public void StartAsync()
        {
            status = AssetAsyncOperationStatus.Loading;
            CreateAsyncOperation();
        }

        public bool IsInLoading() => loadRefCount > 0;
        public void RetainRefCount() => ++loadRefCount;
        public void ReleaseRefCount() => --loadRefCount;
        protected abstract void CreateAsyncOperation();
        public abstract void DoUpdate();
        public abstract UnityObject GetAsset();
        public abstract float Progress();
    }
}
