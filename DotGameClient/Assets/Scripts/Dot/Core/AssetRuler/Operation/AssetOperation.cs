using UnityEngine;

namespace Dot.Core.AssetRuler
{
    public class AssetOperation : ScriptableObject
    {
        public virtual AssetOperationResult Execute(AssetFilterResult filterResult,ref AssetOperationResult operationResult)
        {
            return operationResult;
        }
    }
}
