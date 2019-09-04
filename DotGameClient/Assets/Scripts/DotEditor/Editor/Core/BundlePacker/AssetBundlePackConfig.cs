using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class AssetBundlePackConfig : ScriptableObject
    {
        public enum BundleBuildTarget
        {
            StandaloneWindows64,
            PS4,
            XBoxOne,
        }

        public static readonly string CONFIG_PATH = "Assets/Tools/BundlePack/bundle_pack_config.asset";

        public string bundleOutputDir = "";
        public bool cleanupBeforeBuild = false;
        public BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
        public BundleBuildTarget buildTarget = BundleBuildTarget.StandaloneWindows64;
    }
}
