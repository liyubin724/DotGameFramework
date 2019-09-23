using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetFilter : ScriptableObject
    {
        public virtual bool IsMatch(string assetPath)
        {
            return false;
        }
    }
}
