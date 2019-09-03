using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseAssetSchemaSetting : ScriptableObject
    {
        public string settingName = "Asset Schema Setting";
        public AssetGroupType groupType = AssetGroupType.Addressable;
        public BaseGroupSchema[] groupSchemas = new BaseGroupSchema[0];
    }
}
