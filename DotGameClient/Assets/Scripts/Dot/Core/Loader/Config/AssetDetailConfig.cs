using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.Loader.Config
{
    public class AssetDetailConfig : ScriptableObject
    {
        public List<AssetDetailGroupData> assetGroupDatas = new List<AssetDetailGroupData>();
    }
}
