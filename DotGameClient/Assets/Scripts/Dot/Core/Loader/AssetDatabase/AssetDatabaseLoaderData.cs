namespace Dot.Core.Loader
{
    public class AssetDatabaseLoaderData : AssetLoaderData
    {
        internal AssetDatabaseAsyncOperation[] asyncOperations = null;

        public override void InitData(AssetPathMode pMode)
        {
            base.InitData(pMode);
            asyncOperations = new AssetDatabaseAsyncOperation[assetPaths.Length];
        }

        public override void OnRelease()
        {
            asyncOperations = null;
            base.OnRelease();
        }
    }
}
