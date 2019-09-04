using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.Packer
{
    public class AssetBundlePackConfig : ScriptableObject
    {
        public List<AssetBundleGroupData> groupDatas = new List<AssetBundleGroupData>();
    }
}
