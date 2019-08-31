using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityObject = UnityEngine.Object;
using SystemObject = System.Object;

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
}
