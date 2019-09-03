using System.Collections.Generic;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseGroupSchema : ScriptableObject
    {
        public bool isEnable = true;

        public string groupName = "Asset Group";
        public AssetGroupType groupType = AssetGroupType.Addressable;

        public List<AssetFilterSchema> filters = new List<AssetFilterSchema>();
        public List<BaseActionSchema> actions = new List<BaseActionSchema>();
        
        public AssetFilterResult[] GetFilterResult()
        {
            List<AssetFilterResult> resultList = new List<AssetFilterResult>();
            foreach(var filter in filters)
            {
                if(filter!=null && filter.isEnable)
                {
                    resultList.Add(filter.Execute());
                }
            }
            return resultList.ToArray();
        }

        public virtual AssetExecuteResult Execute(AssetExecuteInput input)
        {
            return null;
        }
    }

    
}
