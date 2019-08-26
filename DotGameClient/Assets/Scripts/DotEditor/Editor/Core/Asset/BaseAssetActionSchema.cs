using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseAssetActionSchema : ScriptableObject
    {
        public string actionName = "Action Scheme";
        public bool isEnable = true;

        public virtual void Execute(AssetGroupActionData actionData)
        {

        }
    }
}
