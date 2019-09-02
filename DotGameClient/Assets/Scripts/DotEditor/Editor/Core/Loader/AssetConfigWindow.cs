using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace DotEditor.Core.Loader
{
    class AssetConfigWindow
    {
        [MenuItem("Test/Build AssetBundle")]
        public static void BuildAsset()
        {
            BuildPipeline.BuildAssetBundles("D:/assets", BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
        }
    }
}
