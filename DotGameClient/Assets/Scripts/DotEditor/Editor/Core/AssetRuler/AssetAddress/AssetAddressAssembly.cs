using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "assetaddress_assembly", menuName = "Asset Ruler/Asset Address/Assembly", order = 1)]
    public class AssetAddressAssembly : AssetAssembly
    {
        public override AssetAssemblyResult Execute()
        {
            AssetAddressAssemblyResult result = new AssetAddressAssemblyResult();
            foreach(var group in assetGroups)
            {
                group.Execute(result);
            }
            return result;
        }
    }
}
