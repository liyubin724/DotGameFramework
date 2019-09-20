using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.AssetRuler.AssetAddress
{
    public class AssetAddressAddressModeOperation : AssetOperation
    {
        public AssetAddressMode addressMode = AssetAddressMode.FileNameWithoutExtension;
        public string fileNameFormat = "{0}";
    }
}
