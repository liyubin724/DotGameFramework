using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    [CreateAssetMenu(fileName = "group_filter_operation", menuName = "Asset Ruler/Asset Address/Group Filter&Operation", order = 1)]
    public class AssetGroupFilterOperation : ScriptableObject
    {
        public bool removeMatchFilterItem = true;

        public AssetComposeType filterComposeType = AssetComposeType.All;
        public List<AssetFilter> assetFilters = new List<AssetFilter>();

        public AssetComposeType operationComposeType = AssetComposeType.All;
        public List<AssetOperation> assetOperations = new List<AssetOperation>();

        private AssetFilterResult ExecuteFilter(AssetSearcherResult searcherResult)
        {
            AssetFilterResult filterResult = new AssetFilterResult();
            for(int i =searcherResult.assetPaths.Count-1;i>=0;--i)
            {
                string assetPath = searcherResult.assetPaths[i];
                if (IsMatchFilter(assetPath))
                {
                    if(removeMatchFilterItem)
                    {
                        searcherResult.assetPaths.RemoveAt(i);
                    }
                    filterResult.assetPaths.Add(assetPath);
                }
            }
            return filterResult;
        }

        private bool IsMatchFilter(string assetPath)
        {
            if(assetFilters == null || assetFilters.Count == 0)
            {
                return true;
            }

            foreach (var filter in assetFilters)
            {
                if (filter == null)
                {
                    continue;
                }

                if (filter.IsMatch(assetPath))
                {
                    if (filterComposeType == AssetComposeType.Any)
                    {
                        return true;
                    }
                }
                else
                {
                    if (filterComposeType == AssetComposeType.All)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public AssetOperationResult[] Execute(AssetSearcherResult searcherResult,AssetGroupResult groupResult)
        {
            AssetFilterResult filterResult = ExecuteFilter(searcherResult);
            List<AssetOperationResult> operationResults = new List<AssetOperationResult>();

            List<AssetOperationResult> results = new List<AssetOperationResult>();
            if (operationComposeType == AssetComposeType.All)
            {
                AssetOperationResult operationResult = null;
                foreach (var assetOperation in assetOperations)
                {
                    if (operationResult == null)
                    {
                        operationResult = assetOperation.Execute(filterResult, null);
                        if (operationResult != null)
                        {
                            results.Add(operationResult);
                        }
                    }
                    else
                    {
                        assetOperation.Execute(filterResult, operationResult);
                    }
                }
            }
            else
            {
                foreach (var assetOperation in assetOperations)
                {
                    AssetOperationResult operationResult = assetOperation.Execute(filterResult, null);
                    if (operationResult != null)
                    {
                        results.Add(operationResult);
                    }
                }
            }

            return results.ToArray();
        }
    }
}
