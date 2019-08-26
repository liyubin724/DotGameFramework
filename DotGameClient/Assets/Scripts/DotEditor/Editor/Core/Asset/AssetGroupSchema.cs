using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    public delegate void BeforeAssetGroupSchemaExecute(AssetGroupSchema group);
    public delegate void AfterAssetGroupSchemaExecute(AssetGroupSchema group);

    public enum AssetGroupType
    {
        Addressable,
        AssetBundle,
        AssetFormat,
    }

    [CreateAssetMenu(fileName = "asset_group_schema", menuName = "Asset/Asset Group Schema")]
    public class AssetGroupSchema : ScriptableObject
    {
        public string groupName = "Group";
        [EnumToggleButtons]
        public AssetGroupType groupType = AssetGroupType.Addressable;
        public bool isEnable = true;
        [ListDrawerSettings(AlwaysAddDefaultValue =true)]
        public List<AssetFilterSchema> filters = new List<AssetFilterSchema>();
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        public List<BaseAssetActionSchema> actions = new List<BaseAssetActionSchema>();

        public BeforeAssetGroupSchemaExecute OnBeforeExecute;
        public AfterAssetGroupSchemaExecute OnAfterExecute;

        [Button(ButtonSizes.Large)]
        public void Execute()
        {
            if (!isEnable) return;

            OnBeforeExecute?.Invoke(this);

            AssetGroupActionData actionData = new AssetGroupActionData();
            actionData.groupName = groupName;
            foreach (var filter in filters)
            {
                if(filter == null || !filter.isEnable)
                {
                    continue;
                }
                AssetGroupFilterData filterData = new AssetGroupFilterData();
                filterData.filterFolder = filter.folder;
                filter.Execute();
                filterData.assets = filter.assets;
                actionData.filterDatas.Add(filterData);
            }
            foreach(var action in actions)
            {
                if(action == null || action.isEnable)
                {
                    action.Execute(actionData);
                }
            }

            OnBeforeExecute?.Invoke(this);
        }
    }

    public class AssetGroupActionData
    {
        public string groupName;
        public List<AssetGroupFilterData> filterDatas = new List<AssetGroupFilterData>();
    }

    public class AssetGroupFilterData
    {
        public string filterFolder;
        public string[] assets;
    }
}
