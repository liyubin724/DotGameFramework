using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "assetaddress_group", menuName = "Asset Ruler/Asset Address/Group", order = 2)]
    public class AssetAddressGroup : AssetGroup
    {
        public bool isMain = true;
        public bool isPreload = false;

        public override void Execute(ref AssetGroupResult groupResult)
        {
            if(groupResult == null)
            {
                groupResult = new AssetAddressGroupResult();
            }
            base.Execute(ref groupResult);

            AssetAddressGroupResult result = groupResult as AssetAddressGroupResult;
            result.groupData = new AssetBundleGroupData();
            result.groupData.groupName = groupName;
            result.groupData.isMain = isMain;
            result.groupData.isPreload = isPreload;

            foreach(var oResult in result.operationResults)
            {
                AssetAddressOperationResult r = oResult as AssetAddressOperationResult;

                foreach(var data in r.addressDataDic.Values)
                {
                    result.groupData.assetDatas.Add(data);
                }
            }
        }
    }
}
