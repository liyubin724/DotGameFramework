using Sirenix.OdinInspector;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseActionSchema : ScriptableObject
    {
        [PropertyOrder(100)]
        public bool isEnable = true;
        [PropertyOrder(100)]
        public string actionName = "Group Name";

        public virtual AssetExecuteResult Execute(AssetExecuteInput inputData)
        {
            return null;
        }
    }
}
