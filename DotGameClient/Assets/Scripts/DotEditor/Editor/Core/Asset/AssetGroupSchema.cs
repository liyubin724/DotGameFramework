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

        public void Execute(Dictionary<string, SystemObject> dataDic)
        {
            if (!isEnable) return;
            if(dataDic == null)
            {
                dataDic = new Dictionary<string, SystemObject>();
            }

            dataDic[AssetSchemaConst.ASSET_GROUP_NAME] = groupName;
            dataDic[AssetSchemaConst.ASSET_FILTER_DATA_NAME] = GetFilterResult();

            foreach(var action in actions)
            {
                action.Execute(dataDic);
            }
        }
        
        [Button(ButtonSizes.Large)]
        public void Execute()
        {
            Execute(null);
        }
    }

    
}
