using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseActionSchema : ScriptableObject
    {
        public bool isEnable = true;
        public string actionName = "Group Name";

        public virtual AssetExecuteResult Execute(AssetExecuteInput inputData)
        {
            return null;
        }
    }
}
