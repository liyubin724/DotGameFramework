using System.Collections.Generic;
using UnityEngine;

namespace Dot.Core.AssetRuler
{
    [CreateAssetMenu(fileName = "asset_compose_filter", menuName = "Asset Ruler/Filter/Compose", order = 100)]
    public class AssetFilterCompose : ScriptableObject
    {
        public AssetComposeType composeType = AssetComposeType.All;
        public List<AssetFilter> assetFilters = new List<AssetFilter>();

        public virtual bool IsMatch(string assetPath)
        {
            foreach(var filter in assetFilters)
            {
                if(filter.IsMatch(assetPath))
                {
                    if(composeType == AssetComposeType.Any)
                    {
                        return true;
                    }
                }else
                {
                    if(composeType == AssetComposeType.All)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
