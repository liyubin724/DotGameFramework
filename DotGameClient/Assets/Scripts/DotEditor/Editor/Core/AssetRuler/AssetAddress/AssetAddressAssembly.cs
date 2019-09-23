using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CreateAssetMenu(fileName = "assetaddress_assembly", menuName = "Asset Ruler/Asset Address/Assembly", order = 1)]
    public class AssetAddressAssembly : AssetAssembly
    {
        public override void Execute()
        {
            AssetAddressAssemblyResult result = new AssetAddressAssemblyResult();
            foreach(var group in assetGroups)
            {
                group.Execute(result);
            }

            int index =0;
        }
    }
}
