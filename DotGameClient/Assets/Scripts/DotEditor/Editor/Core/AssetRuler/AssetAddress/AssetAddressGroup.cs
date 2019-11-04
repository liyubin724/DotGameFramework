using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "assetaddress_group", menuName = "Asset Address/Group", order = 0)]
    public class AssetAddressGroup : AssetGroup
    {
        public bool isGenAddress = false;
        public bool isMain = true;
        public bool isPreload = false;

        protected override AssetGroupResult CreateGroupResult()
        {
            return new AssetAddressGroupResult();
        }

        public override AssetGroupResult Execute(AssetAssemblyResult assemblyResult)
        {
            AssetAddressGroupResult result = base.Execute(assemblyResult) as AssetAddressGroupResult;
            if(result != null)
            {
                result.isGenAddress = isGenAddress;
                result.isMain = isMain;
                result.isPreload = isPreload;
            }
            return result;
        }
    }
}
