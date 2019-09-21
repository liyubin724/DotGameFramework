using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetAssembly : ScriptableObject
    {
        public AssetAssemblyType assetAssemblyType = AssetAssemblyType.AssetAddress;
        public List<AssetGroup> assetGroups = new List<AssetGroup>();

        public virtual void Execute()
        {

        }
    }
}
