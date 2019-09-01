using Priority_Queue;
using System.Collections.Generic;
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
        
        private Dictionary<string, AAssetAsyncOperation> operationDic = new Dictionary<string, AAssetAsyncOperation>();
    }
}
