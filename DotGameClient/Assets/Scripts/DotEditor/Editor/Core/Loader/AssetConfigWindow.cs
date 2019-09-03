using System;
using System.Collections.Generic;
using System.IO;
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
            if (!Directory.Exists("D:/assetbundles"))
            {
                Directory.CreateDirectory("D:/assetbundles");
                //Directory.Delete("D:/assetbundles", true);
            }
            
            BuildPipeline.BuildAssetBundles("D:/assetbundles", BuildAssetBundleOptions.DeterministicAssetBundle|BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        }
    }
}
