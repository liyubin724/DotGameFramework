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
        private bool[] assetLoadStates;
        internal void InitData(AssetPathMode pMode)
        {
            pathMode = pMode;
            assetLoadStates = new bool[assetPaths.Length];
        }

        internal bool GetLoadState(int index) => assetLoadStates[index];

        internal void InvokeComplete(int index,UnityObject uObj)
        {
            string assetPath = GetInvokeAssetPath(index);

            assetLoadStates[index] = true;
            progressCallback?.Invoke(assetPath, 1.0f, userData);
            completeCallback?.Invoke(assetPath, uObj, userData);
        }

        internal void InvokeProgress(int index, float progress) => progressCallback?.Invoke(GetInvokeAssetPath(index), progress, userData);
        internal void InvokeBatchComplete(UnityObject[] uObjs) => batchCompleteCallback?.Invoke(GetInvokeAssetPaths(), uObjs, userData);
        internal void InvokeBatchProgress(float[] progresses) => batchProgressCallback?.Invoke(GetInvokeAssetPaths(), progresses, userData);

        internal void BreakLoader()
        {
            completeCallback = null;
            progressCallback = null;
            batchCompleteCallback = null;
            batchProgressCallback = null;
            userData = null;
        }

        private string GetInvokeAssetPath(int index)
        {
            if (pathMode == AssetPathMode.Address)
            {
                return assetAddresses[index];
            }
            return assetPaths[index];
        }

        private string[] GetInvokeAssetPaths()
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
            assetLoadStates = null;
        }
    }
}
