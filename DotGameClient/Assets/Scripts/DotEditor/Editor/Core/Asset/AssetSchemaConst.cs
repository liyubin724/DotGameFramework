using DotEditor.Core.Packer;

namespace DotEditor.Core.Asset
{
    public class AssetExecuteInput
    {
    }

    public class AssetExecuteResult
    {
    }

    public class AssetFilterResult : AssetExecuteResult
    {
        public string filterFolder;
        public string[] assets;
    }
    
    public class AssetBundleGroupInput : AssetExecuteInput
    {
        public AssetBundlePackConfig packConfig;
    }

    public class AssetBundleActionInput :AssetExecuteInput
    {
        public AssetBundleGroupData groupData;
        public AssetFilterResult[] filterResults;
    }
    



    //------------------------
    
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
        GroupByCount,
    }

    public enum AssetAddressMode
    {
        FullPath,
        FileNameWithoutExtension,
        FileName,
        FileFormatName,
    }
}
