using Dot.Core.Util;
using System.Collections.Generic;
//using UnityEditor.AddressableAssets;
//using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "addressable_create_group", menuName = "Asset/Action/Addressable/Create Group Action")]
    public class AddressableCreateGroupActionSchema : BaseActionSchema
    {
        //public List<AddressableAssetGroupSchema> groupSchemaList = new List<AddressableAssetGroupSchema>();

        public void Execute(AssetGroupActionData actionData)
        {
            if(!isEnable)
            {
                return;
            }

            //AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            //AddressableAssetGroup assetGroup = settings.FindGroup(actionData.groupName);
            //if (assetGroup == null)
            //{
            //    assetGroup = settings.CreateGroup(actionData.groupName, false, false, false, null);
            //}
            //if (groupSchemaList != null && groupSchemaList.Count > 0)
            //{
            //    foreach (var schema in groupSchemaList)
            //    {
            //        if (!assetGroup.HasSchema(schema.GetType()))
            //        {
            //            assetGroup.AddSchema(schema, false);
            //        }
            //    }
            //}
        }
    }
}
