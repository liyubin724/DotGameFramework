using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Core.AssetRuler.AssetAddress
{
    public enum AssetBundlePackMode
    {
        Together,
        Separate,
        GroupByCount,
    }

    public enum AssetAddressMode
    {
        FullPath,
        FileNameWithoutExtension,
        FileName,
        FileFormatName,
    }

    public class AssetAddressOperationResult : AssetOperationResult
    {

    }
}
