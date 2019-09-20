using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Dot.Core.AssetRuler.AssetAddress
{
    public class AssetAddressPackModeOperation : AssetOperation
    {
        public AssetBundlePackMode packMode = AssetBundlePackMode.Together;
        public int packCount = 0;
    }
}
