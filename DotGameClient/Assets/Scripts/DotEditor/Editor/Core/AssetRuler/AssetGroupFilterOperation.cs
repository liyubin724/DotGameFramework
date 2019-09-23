using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    [CreateAssetMenu(fileName = "group_filter_operation", menuName = "Asset Ruler/Asset Address/Group Filter&Operation", order = 1)]
    public class AssetGroupFilterOperation : ScriptableObject
    {
        public AssetComposeType filterComposeType = AssetComposeType.All;
        public List<AssetFilter> assetFilters = new List<AssetFilter>();

        public AssetComposeType operationComposeType = AssetComposeType.All;
        public List<AssetOperation> assetOperations = new List<AssetOperation>();

        public AssetOperationResult[] Execute(AssetSearcherResult searcherResult)
        {
            return null;
            //if(searcherResult == null)
            //{
            //    return null;
            //}

            //AssetFilterResult filterResult = new AssetFilterResult();
            //foreach(var assetPath in searcherResult.assetPaths)
            //{
            //    if(filterCompose == null || filterCompose.IsMatch(assetPath))
            //    {
            //        filterResult.assetPaths.Add(assetPath);
            //        searcherResult.assetPaths.Remove(assetPath);
            //    }
            //}

            //return operationCompose.Execute(filterResult);
        }
    }
}
