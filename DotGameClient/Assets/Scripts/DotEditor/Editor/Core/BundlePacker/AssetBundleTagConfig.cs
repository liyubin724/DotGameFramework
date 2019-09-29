using Dot.Core.Loader.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class AssetBundleTagConfig : ScriptableObject
    {
        public static readonly string CONFIG_PATH = "Assets/Configs/bundle_tag_config.asset";

        public List<AssetBundleGroupData> groupDatas = new List<AssetBundleGroupData>();

        [Serializable]
        public class AssetBundleGroupData
        {
            public string groupName;
            public bool isMain = true;
            public bool isPreload = false;
            public List<AssetAddressData> assetDatas = new List<AssetAddressData>();
        }
    }
}
