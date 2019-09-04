using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class AssetBundleTagConfig : ScriptableObject
    {
        public static readonly string CONFIG_PATH = "Assets/Tools/BundlePack/bundle_tag_config.asset";

        public List<AssetBundleGroupData> groupDatas = new List<AssetBundleGroupData>();
    }
}
