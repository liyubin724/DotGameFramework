using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetOperation : ScriptableObject
    {
        public virtual AssetOperationResult Execute(AssetFilterResult filterResult,AssetOperationResult operationResult)
        {
            return operationResult;
        }
    }
}
