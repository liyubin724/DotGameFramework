using DotEditor.Core.Packer;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "asset_group", menuName = "Asset Schema/Asset Bundle/Group")]
    public class AssetBundleGroupSchema : BaseGroupSchema
    {
        public bool isMain = true;

        public override AssetExecuteResult Execute(AssetExecuteInput input)
        {
            if (!isEnable) return null;
            AssetBundleGroupInput groupInput = input as AssetBundleGroupInput;
            AssetBundleGroupData abGroupData = new AssetBundleGroupData();
            abGroupData.groupName = groupName;
            abGroupData.isMain = isMain;
            groupInput.packConfig.groupDatas.Add(abGroupData);

            AssetBundleActionInput actionInput = new AssetBundleActionInput()
            {
                groupData = abGroupData,
                filterResults = GetFilterResult(),
            };
            foreach (var action in actions)
            {
                action.Execute(actionInput);
            }
            return null;
        }
    }
}
