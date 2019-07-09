using Dot.Core.Manager;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public delegate void OnAssetLoadFinish(int assetIndex, string assetPath, UnityObject obj);
    public delegate void OnAssetLoadProgress(int assetIndex, string assetPath, float progress);

    public delegate void OnBatchAssetLoadFinish(int assetIndex, string[] assetPaths, UnityObject[] objs);
    public delegate void OnBatchAssetLoadProgress(int assetIndex, string[] assetPaths, float[] progress);

    public enum AssetLoaderPriorityType
    {
        VeryLow=0,
        Low,
        Default,
        High,
        VeryHigh,
    }

    public enum AssetType
    {
        Resources=0,
        AssetBundle = 1,
        PackAssetBundle = 2,
    }

    public class AssetManager : IGlobalUpdateManager
    {
        public int Priority { get; set; }

        private AssetType assetType = AssetType.Resources;
        private AAssetLoader assetLoader = null;

        public AssetManager(AssetType assetType)
        {
            this.assetType = assetType;
        }

        public void DoDispose()
        {
            
        }

        public void DoInit()
        {
            
        }

        public void DoReset()
        {
            
        }

        public void DoUpdate(float deltaTime)
        {
            if(assetLoader!=null)
            {
                assetLoader.DoUpdate(deltaTime);
            }
        }

        public void CancelAssetLoader(int index)
        {
            assetLoader.CancelAssetLoader(index);
        }

        public UnityObject LoadAsset(string assetFullPath)
        {
            return assetLoader.LoadAsset(assetFullPath);
        }

        public UnityObject[] LoadBatchAsset(string[] assetFullPaths)
        {
            return assetLoader.LoadBatchAsset(assetFullPaths);
        }

        public int LoadAssetAsync(string assetFullPath,OnAssetLoadFinish finishCallback,OnAssetLoadProgress progressCallback = null,AssetLoaderPriorityType priorityType = AssetLoaderPriorityType.Default)
        {
            AssetAsyncData data = new AssetAsyncData(assetFullPath, finishCallback, progressCallback, priorityType);
            return assetLoader.LoadAssetAsync(data);
        }

        public int LoadBatchAssetAsync(string[] assetFullPaths,OnBatchAssetLoadFinish finishCallback,OnBatchAssetLoadProgress progressCallback=null,AssetLoaderPriorityType priorityType = AssetLoaderPriorityType.Default)
        {
            BatchAssetAsyncData data = new BatchAssetAsyncData(assetFullPaths, finishCallback, progressCallback, priorityType);
            return assetLoader.LoadBatchAssetAsync(data);
        }

        public void UnloadUnsedAsset()
        {
            assetLoader.UnloadUnusedAsset();
        }

    }
}
