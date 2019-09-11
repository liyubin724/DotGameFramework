using Dot.Core.Pool;
using Priority_Queue;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetLoaderData : FastPriorityQueueNode,IObjectPoolItem
    {
        public long uniqueID = -1;
        public string[] assetPaths;
        public string[] assetAddresses;

        public bool isInstance = false;
        public SystemObject userData;

        public OnAssetLoadComplete completeCallback;
        public OnAssetLoadProgress progressCallback;
        public OnBatchAssetLoadComplete batchCompleteCallback;
        public OnBatchAssetsLoadProgress batchProgressCallback;
        
        private AssetPathMode pathMode;
        public void InitData(AssetPathMode pMode)
        {
            pathMode = pMode;
        }

        public void InvokeComplete(int index, UnityObject uObj)=> completeCallback?.Invoke(GetInvokeAssetPath(index), uObj, userData);
        public void InvokeProgress(int index, float progress) => progressCallback?.Invoke(GetInvokeAssetPath(index), progress, userData);
        public void InvokeBatchComplete(UnityObject[] uObjs) => batchCompleteCallback?.Invoke(GetInvokeAssetPaths(), uObjs, userData);
        public void InvokeBatchProgress(float[] progresses) => batchProgressCallback?.Invoke(GetInvokeAssetPaths(), progresses, userData);

        private string GetInvokeAssetPath(int index)
        {
            if (pathMode == AssetPathMode.Address)
            {
                return assetAddresses[index];
            }
            return assetPaths[index];
        }

        public string[] GetInvokeAssetPaths()
        {
            if(pathMode == AssetPathMode.Address)
            {
                return assetAddresses;
            }
            return assetPaths;
        }
       
        public void OnNew() { }

        public void OnRelease()
        {
            uniqueID = -1;
            assetPaths = null;
            completeCallback = null;
            progressCallback = null;
            batchCompleteCallback = null;
            batchProgressCallback = null;
            isInstance = false;
            userData = null;
        }
    }
}
