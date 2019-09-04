using Dot.Core.Loader.Config;
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
            AssetDetailGroupData detailGroupData = new AssetDetailGroupData();
            detailGroupData.groupName = groupName;
            detailGroupData.isMain = isMain;
            groupInput.detailConfig.assetGroupDatas.Add(detailGroupData);

            AssetBundleActionInput actionInput = new AssetBundleActionInput()
            {
                detailGroupData = detailGroupData,
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
