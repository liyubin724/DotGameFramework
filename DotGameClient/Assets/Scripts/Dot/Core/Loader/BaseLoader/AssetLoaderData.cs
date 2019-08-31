using Dot.Core.Generic;
using SystemObject = System.Object;

namespace Dot.Core.Loader
{
    public class AssetLoaderData
    {
        public long uniqueID = -1;
        public string[] assetPaths;

        public OnAssetLoadComplete completeCallback;
        public OnAssetLoadProgress progressCallback;
        public OnBatchAssetLoadComplete batchCompleteCallback;
        public OnBatchAssetsLoadProgress batchProgressCallback;

        public bool isInstance = false;

        public SystemObject userData;

        public string AssetPath
        {
            get
            {
                if(assetPaths!=null && assetPaths.Length>0)
                {
                    return assetPaths[0];
                }
                return null;
            }
        }
    }
}
