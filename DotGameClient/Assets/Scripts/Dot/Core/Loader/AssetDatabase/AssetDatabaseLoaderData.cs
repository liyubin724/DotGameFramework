using Dot.Core.Pool;

namespace Dot.Core.Loader
{
    public class AssetDatabaseLoaderData : AssetLoaderData, IObjectPoolItem
    {
        internal AssetDatabaseAsyncOperation[] asyncOperations = null;
        internal void Init()
        {
            asyncOperations = new AssetDatabaseAsyncOperation[assetPaths.Length];
        }

        public void OnNew()
        {
        }

        public void OnRelease()
        {
            Reset();
        }
    }
}
