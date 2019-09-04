using System;
using System.Collections.Generic;

namespace Dot.Core.Loader.Config
{
    public class AssetDetailConst
    {
        public static readonly string ASSET_DETAIL_CONFIG_PATH = "Assets/Configs/AssetDetail/asset_detail_config.asset";
    }

    [Serializable]
    public class AssetDetailData
    {
        public string address;
        public string path;
        public string bundle;
        public string[] labels = new string[0];
    }
    
    [Serializable]
    public class AssetDetailGroupData
    {
        public string groupName;
        public bool isMain = true;
        public List<AssetDetailData> assetDetailDatas = new List<AssetDetailData>();
    }
}
