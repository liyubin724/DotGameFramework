using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public delegate void OnAssetLoadComplete(string pathOrAddress, UnityObject uObj, SystemObject userData);
    public delegate void OnAssetLoadProgress(string pathOrAddress, float progress, SystemObject userData);

    public delegate void OnBatchAssetLoadComplete(string[] pathOrAddresses, UnityObject[] uObjs, SystemObject userData);
    public delegate void OnBatchAssetsLoadProgress(string[] pathOrAddresses, float[] progresses, SystemObject userData);

    public delegate void OnSceneComplete(string pathOrAddress,SystemObject userData);
    public delegate void OnSceneProgress(string pathOrAddress, float progress, SystemObject userData);

    public enum AssetLoaderMode
    {
        AssetDatabase,
        Resources,
        AssetBundle,
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

    public enum AssetLoaderState
    {
        None,
        Waiting,
        Loading,
        Complete,
        Cancel,
    }
}
