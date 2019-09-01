using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public delegate void OnAssetLoadComplete(string assetPath, UnityObject uObj, SystemObject userData);
    public delegate void OnAssetLoadProgress(string assetPath, float progress);

    public delegate void OnBatchAssetLoadComplete(string[] assetPaths, UnityObject[] uObjs, SystemObject userData);
    public delegate void OnBatchAssetsLoadProgress(string[] assetPaths, float[] progresses);

    public enum AssetLoaderMode
    {
        AssetDatabase,
        Resources,
        AssetBundle,
        PackedAssetBundle,
    }

    public enum AssetLoaderPriority
    {
        VeryLow = 100,
        Low = 200,
        Default = 300,
        High = 400,
        VeryHigh = 500,
    }
}
