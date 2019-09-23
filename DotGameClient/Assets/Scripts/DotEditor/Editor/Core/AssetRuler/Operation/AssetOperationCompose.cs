using System;
using System.Collections.Generic;

namespace DotEditor.Core.AssetRuler
{
    [Serializable]
    public class AssetOperationCompose
    {
        public AssetComposeType composeType = AssetComposeType.All;
        public List<AssetOperation> assetOperations = new List<AssetOperation>();

        public virtual AssetOperationResult[] Execute(AssetFilterResult filterResult)
        {
            List<AssetOperationResult> results = new List<AssetOperationResult>();
            if(composeType == AssetComposeType.All)
            {
                AssetOperationResult operationResult = null;
                foreach (var assetOperation in assetOperations)
                {
                    if(operationResult == null)
                    {
                        operationResult = assetOperation.Execute(filterResult, ref operationResult);
                        if(operationResult!=null)
                        {
                            results.Add(operationResult);
                        }
                    }
                    else
                    {
                        assetOperation.Execute(filterResult, ref operationResult);
                    }
                }
            }else
            {
                foreach (var assetOperation in assetOperations)
                {
                    AssetOperationResult operationResult = null;
                    assetOperation.Execute(filterResult, ref operationResult);
                    if(operationResult!=null)
                    {
                        results.Add(operationResult);
                    }
                }
            }

            return results.ToArray();
        }
    }
}
