using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityObject = UnityEngine.Object;

namespace Dot.Core.Loader
{
    public class AssetObjectData
    {
        public string assetPath;
        public WeakReference assetWeakRef = null;

        public AssetObjectData() { }
        
    }
}
