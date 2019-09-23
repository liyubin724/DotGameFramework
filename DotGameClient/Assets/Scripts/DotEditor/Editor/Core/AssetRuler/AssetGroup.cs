using DotEditor.Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetGroup : ScriptableObject
    {
        public bool isEnable = true;
        public string groupName = "Asset Group";
        public AssetAssemblyType assetAssemblyType = AssetAssemblyType.AssetAddress;

        public AssetSearcher assetSearcher = new AssetSearcher();
        public List<AssetGroupFilterOperation> filterOperations = new List<AssetGroupFilterOperation>();

        public virtual void Execute(ref AssetGroupResult groupResult)
        {
            if(groupResult == null)
            {
                Debug.LogError("");
                return;
            }
            if(!isEnable ||  filterOperations.Count == 0)
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
    }
}
