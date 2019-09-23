using System;
using System.Collections.Generic;

namespace DotEditor.Core.AssetRuler
{
    [Serializable]
    public class AssetFilterCompose
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
