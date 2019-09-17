using Dot.Core.Pool;
using Priority_Queue;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetLoaderData : FastPriorityQueueNode,IObjectPoolItem
    {
        public long uniqueID = -1;
        public string[] pathOrAddresses;
        public string[] assetPaths;

        public bool isInstance = false;
        public SystemObject userData;

        public OnAssetLoadComplete completeCallback;
        public OnAssetLoadProgress progressCallback;
        public OnBatchAssetLoadComplete batchCompleteCallback;
        public OnBatchAssetsLoadProgress batchProgressCallback;
        
        private bool[] assetLoadStates;
        internal void InitData()
        {
            assetLoadStates = new bool[assetPaths.Length];
        }

        internal bool GetLoadState(int index) => assetLoadStates[index];

        internal void InvokeComplete(int index,UnityObject uObj)
        {
            string pathOrAddress = pathOrAddresses[index];

            assetLoadStates[index] = true;
            progressCallback?.Invoke(pathOrAddress, 1.0f, userData);
            completeCallback?.Invoke(pathOrAddress, uObj, userData);
        }

        internal void InvokeProgress(int index, float progress) => progressCallback?.Invoke(pathOrAddresses[index], progress, userData);
        internal void InvokeBatchComplete(UnityObject[] uObjs) => batchCompleteCallback?.Invoke(pathOrAddresses, uObjs, userData);
        internal void InvokeBatchProgress(float[] progresses) => batchProgressCallback?.Invoke(pathOrAddresses, progresses, userData);

        internal void BreakLoader()
        {
            completeCallback = null;
            progressCallback = null;
            batchCompleteCallback = null;
            batchProgressCallback = null;
            userData = null;
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
