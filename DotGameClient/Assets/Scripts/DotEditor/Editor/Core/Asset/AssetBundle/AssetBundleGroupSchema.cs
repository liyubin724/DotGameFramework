using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "asset_group", menuName = "Asset Schema/Asset Bundle/Group")]
    public class AssetBundleGroupSchema : BaseGroupSchema
    {
        public override AssetExecuteResult Execute(AssetExecuteInput input)
        {
            if (!isEnable) return null;
            AssetBundleGroupInput groupInput = input as AssetBundleGroupInput;
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
    }
}
