using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetOperationCompose : ScriptableObject
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
