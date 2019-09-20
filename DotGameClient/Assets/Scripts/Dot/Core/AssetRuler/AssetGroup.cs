using Dot.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.AssetRuler
{
    [CreateAssetMenu(fileName = "asset_group", menuName = "Asset Ruler/Group")]
    public class AssetGroup : ScriptableObject
    {
        public bool isEnable = true;
        public string groupName = "Asset Group";
        public AssetAssemblyType assetAssemblyType = AssetAssemblyType.AssetAddress;
        public AssetSearcher assetSearcher = null;
        public List<AssetGroupFilterOperation> filterOperations = new List<AssetGroupFilterOperation>();

        public virtual void Execute(ref AssetGroupResult groupResult)
        {
            if(!isEnable || assetSearcher == null || filterOperations.Count == 0)
            {
                return;
            }
            groupResult.groupName = groupName;

            AssetSearcherResult searcherResult = assetSearcher.Execute();
            foreach(var filterOperation in filterOperations)
            {
                AssetOperationResult[] operationResults = filterOperation.Execute(searcherResult);
                if(operationResults != null)
                {
                    groupResult.operationResults.AddRange(operationResults);
                }
            }
        }

        [Serializable]
        public class AssetSearcher
        {
            public string folder = "Assets";
            public bool includeSubfolder = true;
            public string fileNameFilterRegex = "";

            public AssetSearcherResult Execute()
            {
                string[] assets = DirectoryUtil.GetAssetsByFileNameFilter(folder, includeSubfolder, fileNameFilterRegex, new string[] { ".meta" });
                AssetSearcherResult result = new AssetSearcherResult();
                result.assetPaths.AddRange(assets);
                return result;
            }
        }

        [Serializable]
        public class AssetGroupFilterOperation
        {
            public AssetFilterCompose filterCompose = null;
            public AssetOperationCompose operationCompose = null;

            public AssetOperationResult[] Execute(AssetSearcherResult searcherResult)
            {
                if(operationCompose == null || searcherResult == null)
                {
                    return null;
                }

                AssetFilterResult filterResult = new AssetFilterResult();
                foreach(var assetPath in searcherResult.assetPaths)
                {
                    if(filterCompose == null || filterCompose.IsMatch(assetPath))
                    {
                        filterResult.assetPaths.Add(assetPath);
                        searcherResult.assetPaths.Remove(assetPath);
                    }
                }

                return operationCompose.Execute(filterResult);
            }
        }
    }
}
