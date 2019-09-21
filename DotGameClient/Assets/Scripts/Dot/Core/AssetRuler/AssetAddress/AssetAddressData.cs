using Dot.Core.Loader.Config;
using System;
using System.Collections.Generic;

namespace Dot.Core.AssetRuler.AssetAddress
{
    public enum AssetBundlePackMode
    {
        Together,
        Separate,
        GroupByCount,
    }

    public enum AssetAddressMode
    {
        FullPath,
        FileNameWithoutExtension,
        FileName,
        FileFormatName,
    }

    public class AssetAddressOperationResult : AssetOperationResult
    {
        public Dictionary<string, AssetAddressData> addressDataDic = new Dictionary<string, AssetAddressData>();
    }

    public class AssetAddressGroupResult : AssetGroupResult
    {
        public AssetBundleGroupData groupData;
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
