using Priority_Queue;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetLoaderData : FastPriorityQueueNode
    {
        public long uniqueID = -1;
        public string[] assetPaths;
        public string[] assetAddresses;

        public OnAssetLoadComplete completeCallback;
        public OnAssetLoadProgress progressCallback;
        public OnBatchAssetLoadComplete batchCompleteCallback;
        public OnBatchAssetsLoadProgress batchProgressCallback;

        public bool isInstance = false;
        public SystemObject userData;

        private bool[] isCompleteCalled = null;
        private AssetPathMode pathMode;

        public virtual void InitData(AssetPathMode pMode)
        {
            pathMode = pMode;
            isCompleteCalled = new bool[assetPaths.Length];
            for (int i = 0; i < assetPaths.Length; ++i)
            {
                isCompleteCalled[i] = false;
            }
        }

        public bool GetIsCompleteCalled(int index) => isCompleteCalled[index];

        public void InvokeComplete(int index, UnityObject uObj)
        {
            isCompleteCalled[index] = true;
            completeCallback?.Invoke(GetInvokeAssetPath(index), uObj, userData);
        }
        public void InvokeProgress(int index, float progress) => progressCallback?.Invoke(GetInvokeAssetPath(index), progress);
        public void InvokeBatchComplete(UnityObject[] uObjs) => batchCompleteCallback?.Invoke(GetInvokeAssetPaths(), uObjs, userData);
        public void InvokeBatchProgress(float[] progresses) => batchProgressCallback?.Invoke(GetInvokeAssetPaths(), progresses);

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
        
        protected void Reset()
        {
            uniqueID = -1;
            assetPaths = null;
            completeCallback = null;
            progressCallback = null;
            batchCompleteCallback = null;
            batchProgressCallback = null;
            isInstance = false;
            userData = null;
            isCompleteCalled = null;
        }
    }
}
