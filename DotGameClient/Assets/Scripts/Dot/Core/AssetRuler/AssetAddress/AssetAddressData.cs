using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Dictionary<string, AssetBundleAddressData> addressDataDic = new Dictionary<string, AssetBundleAddressData>();
    }

    [Serializable]
    public class AssetBundleAddressData
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
        public List<AssetBundleAddressData> assetDatas = new List<AssetBundleAddressData>();
    }
}
