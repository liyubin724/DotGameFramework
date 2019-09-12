using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public delegate void OnAssetLoadComplete(string assetPath, UnityObject uObj, SystemObject userData);
    public delegate void OnAssetLoadProgress(string assetPath, float progress, SystemObject userData);

    public delegate void OnBatchAssetLoadComplete(string[] assetPaths, UnityObject[] uObjs, SystemObject userData);
    public delegate void OnBatchAssetsLoadProgress(string[] assetPaths, float[] progresses, SystemObject userData);

    public delegate void OnSceneLoadComplete(string assetPath,SystemObject userData);
    public delegate void OnSceneLoadProgress(string assetPath, float progress, SystemObject userData);
    public delegate void OnSceneUnloadComplete(string assetPath, SystemObject userData);
    public delegate void OnSceneUnloadProgress(string assetPath, float progress, SystemObject userData);

    public enum AssetLoaderMode
    {
        AssetDatabase,
        Resources,
        AssetBundle,
        PackedAssetBundle,
    }

    public enum AssetPathMode
    {
        Address,
        Path,
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
