using Dot.Core.Loader;
using System;
using UnityEditor;

namespace DotEditor.Core.Packer
{
    public enum CompressOptions
    {
        Uncompressed = 0,
        StandardCompression,
        ChunkBasedCompression,
    }

    public enum ValidBuildTarget
    {
        PS3 = 10,
        XBOX360 = 11,
        StandaloneWindows64 = 19,
        PSP2 = 30,
        PS4 = 31,
        PSM = 32,
        XboxOne = 33,
    }

    [Serializable] 
    public class BundlePackConfig
    {
        public string outputDirPath = "D:/assetbundle";
        public bool cleanupBeforeBuild = false;
        public ValidBuildTarget buildTarget = ValidBuildTarget.StandaloneWindows64;
        public CompressOptions compression = CompressOptions.StandardCompression;

        public BuildAssetBundleOptions bundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle;

        internal BuildTarget GetBuildTarget()
        {
            return (BuildTarget)buildTarget;
        }

        internal BuildAssetBundleOptions GetBundleOptions()
        {
            if(compression == CompressOptions.Uncompressed)
            {
                return bundleOptions | BuildAssetBundleOptions.UncompressedAssetBundle;
            }else if(compression == CompressOptions.ChunkBasedCompression)
            {
                return bundleOptions | BuildAssetBundleOptions.ChunkBasedCompression;
            }else
            {
                return bundleOptions;
            }
        }
    }
}
