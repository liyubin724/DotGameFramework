using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Loader.Config
{
    public class AssetDetailConfig : ScriptableObject
    {
        [ReadOnly]
        public List<AssetDetailGroupData> assetGroupDatas = new List<AssetDetailGroupData>();
    }
}
