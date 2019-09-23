using Dot.Core.Loader.Config;
using System;
using System.Collections.Generic;

namespace DotEditor.Core.AssetRuler.AssetAddress
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

    public class AssetAddressAssemblyResult : AssetAssemblyResult
    {

    }

    public class AssetAddressGroupResult : AssetGroupResult
    {
        public bool isMain = true;
        public bool isPreload = false;
    }
    
    public class AssetAddressOperationResult : AssetOperationResult
    {
        public Dictionary<string, AssetAddressData> addressDataDic = new Dictionary<string, AssetAddressData>();
    }

   
}
