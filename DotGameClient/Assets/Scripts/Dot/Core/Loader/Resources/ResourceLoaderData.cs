using Dot.Core.Pool;

namespace Dot.Core.Loader
{
    public class ResourceLoaderData : AssetLoaderData, IObjectPoolItem
    {
        internal ResourceAsyncOperation[] asyncOperations = null;

        public override void InitData(AssetPathMode pathMode)
        {
            base.InitData(pathMode);
            asyncOperations = new ResourceAsyncOperation[assetPaths.Length];
        }

        public void OnNew()
        {
            
        }

        public void OnRelease()
        {
            Reset();
            asyncOperations = null;
        }
    }
}
