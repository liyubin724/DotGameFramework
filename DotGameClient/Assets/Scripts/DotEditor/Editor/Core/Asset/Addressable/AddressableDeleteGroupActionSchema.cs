using Dot.Core.Util;
//using UnityEditor.AddressableAssets;
//using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "addressable_delete_group", menuName = "Asset/Action/Addressable/Delete Group Action")]
    public class AddressableDeleteGroupActionSchema : BaseAssetActionSchema
    {
        public override void Execute(AssetGroupActionData actionData)
        {
            if(!isEnable)
            {
                return;
            }
            //AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            //AddressableAssetGroup assetGroup = settings.FindGroup(actionData.groupName);
            //if(assetGroup!=null)
            //{
            //    settings.RemoveGroup(assetGroup);
            //}
        }
    }
}
