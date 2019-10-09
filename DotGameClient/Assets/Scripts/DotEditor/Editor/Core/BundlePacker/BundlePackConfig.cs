using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class BundlePackConfig
    {
        public enum BundleBuildTarget
        {
            StandaloneWindows64,
            PS4,
            XBoxOne,
        }

        public string bundleOutputDir = "D:/assetbundle";
        public bool cleanupBeforeBuild = false;
        public BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
        public BundleBuildTarget buildTarget = BundleBuildTarget.StandaloneWindows64;
    }
}
