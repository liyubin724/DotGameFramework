using System;
using System.Collections.Generic;

namespace DotEditor.Core.Packer
{
    public class AssetBundlePackConst
    {
        public static readonly string ASSET_BUNDLE_PACK_CONFIG_PATH = "Assets/Tools/BundlePack/bundle_pack_config.asset";
    }

    [Serializable]
    public class AssetBundleAssetData
    {
        public string address;
        public string path;
        public string bundle;
        public string[] labels = new string[0];
    }
    
    [Serializable]
    public class AssetBundleGroupData
    {
        public string groupName;
        public bool isMain = true;
        public List<AssetBundleAssetData> assetDatas = new List<AssetBundleAssetData>();
    }
}
