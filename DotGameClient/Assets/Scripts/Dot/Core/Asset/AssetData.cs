using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Asset
{
    public enum AssetDataStatus
    {
        None,
        Loading,
        Loaded,
    }
    public class AssetData
    {
        public AsyncOperationHandle Handle { get; set; }

        public string Address { get; set; }
        public AssetDataStatus Status { get; set; } = AssetDataStatus.None;

        private int refCount = 0;
        internal void RetainRefCount() => ++refCount;
        internal void ReleaseRefCount() => --refCount;

        private int loadCount = 0;
        internal void RetainLoadCount() => ++loadCount;
        internal void ReleaseLoadCount() => --loadCount;
        
        public bool IsValid()
        {
            if(refCount>0 || loadCount>0)
            {
                return true;
            }
            return false;
        }

        public UnityObject GetObject()
        {
            if(Status == AssetDataStatus.Loaded && Handle.Status == AsyncOperationStatus.Succeeded)
            {
                return (UnityObject)Handle.Result;
            }
            return null;
        }

        public void Unload()
        {
            Addressables.Release(Handle);
        }
    }
}
