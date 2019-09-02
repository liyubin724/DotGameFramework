using Priority_Queue;
using SystemObject = System.Object;

namespace Dot.Core.Loader
{
    public class AssetLoaderData : FastPriorityQueueNode
    {
        public long uniqueID = -1;
        public string[] assetPaths;

        public OnAssetLoadComplete completeCallback;
        public OnAssetLoadProgress progressCallback;
        public OnBatchAssetLoadComplete batchCompleteCallback;
        public OnBatchAssetsLoadProgress batchProgressCallback;

        public bool isInstance = false;
        public SystemObject userData;

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
        }
    }
}
