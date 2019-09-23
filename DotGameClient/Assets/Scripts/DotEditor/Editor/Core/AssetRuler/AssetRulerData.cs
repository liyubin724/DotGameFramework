using System.Collections.Generic;

namespace DotEditor.Core.AssetRuler
{
    public enum AssetAssemblyType
    {
        AssetAddress,
    }

    public enum AssetComposeType
    {
        All,
        Any,
    }

    public class AssetAssemblyResult
    {
        public List<AssetGroupResult> groupResults = new List<AssetGroupResult>();
    }

    public class AssetSearcherResult
    {
        public List<string> assetPaths = new List<string>();
    }

    public class AssetGroupResult
    {
        public string groupName = "";
        public List<AssetOperationResult> operationResults = new List<AssetOperationResult>();
    }
    
    public class AssetFilterResult
    {
        public List<string> assetPaths = new List<string>();
    }

    public class AssetOperationResult
    {
    }

    
}
