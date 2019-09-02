using UnityEngine;
using SystemObject = System.Object;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace DotEditor.Core.Asset
{
    public class BaseActionSchema : ScriptableObject
    {
        [PropertyOrder(100)]
        public bool isEnable = true;
        [PropertyOrder(100)]
        public string actionName = "Group Name";

        public virtual void Execute(Dictionary<string,SystemObject> dataDic)
        {
            
        }
    }
}
