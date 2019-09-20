using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.AssetRuler
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

    public class AssetSearcherResult
    {
        public List<string> assetPaths = new List<string>();
    }

    public class AssetFilterResult
    {
        public List<string> assetPaths = new List<string>();
    }

    public class AssetOperationResult
    {
    }

    public class AssetGroupResult
    {
        public string groupName = "";
        public List<AssetOperationResult> operationResults = new List<AssetOperationResult>();

        public void CopyTo(AssetGroupResult gResult)
        {
            gResult.groupName = groupName;
            gResult.operationResults.AddRange(operationResults);
        }
    }
}
