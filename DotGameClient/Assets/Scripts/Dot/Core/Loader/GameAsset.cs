using Dot.Core.Manager;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public static class GameAsset
    {
        public static void CancelAssetLoader(int index)
        {
            GlobalManager.GetInstance().AssetMgr.CancelAssetLoader(index);
        }

        public static UnityObject LoadAsset(string assetFullPath)
        {
            return GlobalManager.GetInstance().AssetMgr.LoadAsset(assetFullPath);
        }

        public static UnityObject[] LoadBatchAsset(string[] assetFullPaths)
        {
            return GlobalManager.GetInstance().AssetMgr.LoadBatchAsset(assetFullPaths);
        }

        public static int LoadAssetAsync(string assetFullPath, OnAssetLoadFinish finishCallback, OnAssetLoadProgress progressCallback = null, AssetLoaderPriorityType priorityType = AssetLoaderPriorityType.Default)
        {
            return GlobalManager.GetInstance().AssetMgr.LoadAssetAsync(assetFullPath, finishCallback, progressCallback, priorityType);
        }

        public static int LoadBatchAssetAsync(string[] assetFullPaths, OnBatchAssetLoadFinish finishCallback, OnBatchAssetLoadProgress progressCallback = null, AssetLoaderPriorityType priorityType = AssetLoaderPriorityType.Default)
        {
            return GlobalManager.GetInstance().AssetMgr.LoadBatchAssetAsync(assetFullPaths, finishCallback, progressCallback, priorityType);
        }

        public static void UnloadUnusedAsset()
        {
            GlobalManager.GetInstance().AssetMgr.UnloadUnsedAsset();
        }

        public static void UnloadAllAsset()
        {

        }

    }
}
