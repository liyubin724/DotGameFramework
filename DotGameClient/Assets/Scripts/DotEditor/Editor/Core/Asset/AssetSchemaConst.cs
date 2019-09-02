using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotEditor.Core.Asset
{
    public static class AssetSchemaConst
    {
        public static readonly string ASSET_GROUP_NAME = "groupName";
        public static readonly string ASSET_FILTER_DATA_NAME = "filterResult";
    }

    public class AssetFilterResult
    {
        public string filterFolder;
        public string[] assets;
    }

    public enum AssetGroupType
    {
        Addressable,
        AssetBundle,
        AssetFormat,
    }

    public class AssetGroupActionData
    {
        public string groupName;
        public AssetFilterResult[] filterResults = new AssetFilterResult[0];
    }

    public enum AssetBundlePackMode
    {
        Together,
        Separate,
    }

    public enum AssetAddressMode
    {
        FullPath,
        FileNameWithoutExtension,
        FileName,
        FileFormatName,
    }
}
