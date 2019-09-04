using Dot.Core.Loader.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public AssetDetailConfig detailConfig;
    }

    public class AssetBundleActionInput :AssetExecuteInput
    {
        public AssetDetailGroupData detailGroupData;
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
