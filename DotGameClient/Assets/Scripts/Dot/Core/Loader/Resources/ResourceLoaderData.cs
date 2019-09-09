namespace Dot.Core.Loader
{
    public class ResourceLoaderData : AssetLoaderData
    {
        internal ResourceAsyncOperation[] asyncOperations = null;

        public override void InitData(AssetPathMode pathMode)
        {
            base.InitData(pathMode);

            asyncOperations = new ResourceAsyncOperation[assetPaths.Length];
        }

        public override void OnRelease()
        {
            asyncOperations = null;
            base.OnRelease();
        }
    }
}
