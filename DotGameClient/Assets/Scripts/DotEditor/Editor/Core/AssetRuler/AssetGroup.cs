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

        public List<AssetSearcher> assetSearchers = new List<AssetSearcher>();
        public List<AssetGroupFilterOperation> filterOperations = new List<AssetGroupFilterOperation>();

        protected virtual AssetGroupResult CreateGroupResult()
        {
            return new AssetGroupResult();
        }

        public virtual AssetGroupResult Execute(AssetAssemblyResult assemblyResult)
        {
            if(assemblyResult == null)
            {
                Debug.LogError("AssetGroup::Execute->Assembly Result is Null");
                return null;
            }
            if(!isEnable ||  filterOperations.Count == 0)
            {
                return null;
            }

            AssetSearcherResult searcherResult = new AssetSearcherResult();//assetSearcher.Execute();
            foreach(var searcher in assetSearchers)
            {
                searcherResult.assetPaths.AddRange(searcher.Execute().assetPaths);
            }

            AssetGroupResult groupResult = CreateGroupResult();
            groupResult.groupName = groupName;
            assemblyResult.groupResults.Add(groupResult);

            foreach (var filterOperation in filterOperations)
            {
                AssetOperationResult[] operationResults = filterOperation.Execute(searcherResult,groupResult);
                if(operationResults != null)
                {
                    groupResult.operationResults.AddRange(operationResults);
                }
            }

            return groupResult;
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
