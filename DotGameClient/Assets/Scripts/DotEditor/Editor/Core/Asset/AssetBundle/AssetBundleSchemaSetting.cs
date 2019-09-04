using DotEditor.Core.Packer;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "asset_setting", menuName = "Asset Schema/Asset Bundle/Setting")]
    public class AssetBundleSchemaSetting :  BaseAssetSchemaSetting
    {
        public AssetBundlePackConfig packConfig;
    }
}
