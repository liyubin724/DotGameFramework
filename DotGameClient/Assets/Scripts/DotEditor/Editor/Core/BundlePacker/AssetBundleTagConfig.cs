using Dot.Core.Loader.Config;
using System;
using System.Collections.Generic;

namespace DotEditor.Core.Packer
{
    [Serializable]
    public class AssetBundleTagConfig
    {
        public List<AssetBundleGroupData> groupDatas = new List<AssetBundleGroupData>();
    }

    [Serializable]
    public class AssetBundleGroupData
    {
        public string groupName;
        public bool isMain = true;
        public bool isPreload = false;
        public List<AssetAddressData> assetDatas = new List<AssetAddressData>();
    }
}
