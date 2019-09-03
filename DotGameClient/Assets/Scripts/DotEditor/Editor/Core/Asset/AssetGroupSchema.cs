using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using SystemObject = System.Object;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "asset_group_schema", menuName = "Asset/Asset Group Schema")]
    public class AssetGroupSchema : ScriptableObject
    {
        public bool isEnable = true;

        public string groupName = "Asset Group";

        [EnumToggleButtons]
        public AssetGroupType groupType = AssetGroupType.Addressable;

        [ListDrawerSettings(AlwaysAddDefaultValue =true,ShowIndexLabels =true,Expanded =true)]
        public List<AssetFilterSchema> filters = new List<AssetFilterSchema>();

        [ListDrawerSettings(AlwaysAddDefaultValue = true, ShowIndexLabels = true, Expanded = true)]
        public List<BaseActionSchema> actions = new List<BaseActionSchema>();

        public AssetFilterResult[] GetFilterResult()
        {
            List<AssetFilterResult> resultList = new List<AssetFilterResult>();
            foreach(var filter in filters)
            {
                if(filter!=null && filter.isEnable)
                {
                    resultList.Add(filter.Execute());
                }
            }
            return resultList.ToArray();
        }

        private AssetExecuteResult ExecuteAssetBundle(AssetBundleGroupInput groupInput)
        {
            AssetBundleActionInput actionInput = new AssetBundleActionInput()
            {
                groupName = groupName,
                filterResults = GetFilterResult()
            };
            foreach (var action in actions)
            {
                AssetBundleActionResult result = action.Execute(actionInput) as AssetBundleActionResult;
                groupInput.detailConfig.assetGroupDatas.Add(result.groupData);
            }
            return null;
        }

        public AssetExecuteResult Execute(AssetExecuteInput inputData)
        {
            if (!isEnable) return null;

            if(groupType == AssetGroupType.AssetBundle)
            {
                return ExecuteAssetBundle(inputData as AssetBundleGroupInput);
            }
            
            return null;
        }
        
        //[Button(ButtonSizes.Large)]
        //public void Execute()
        //{
        //    Execute(null);
        //}
    }

    
}
