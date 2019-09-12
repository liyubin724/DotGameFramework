//using DotEditor.Core.Util;
//using Sirenix.OdinInspector;
//using System;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace DotEditor.Core.Asset
//{
//    [CreateAssetMenu(fileName = "addressable_asset_setting", menuName = "Asset/Asset Setting/Addressable")]
//    public class AddressableSchemaSetting : BaseAssetSchemaSetting
//    {
//        [PropertyOrder(300)]
//        [Button("Execute Addressable", ButtonSizes.Large)]
//        public void ExecuteAddressable()
//        {
//            if (groupSchemas != null && groupSchemas.Length > 0)
//            {
//                Array.ForEach(groupSchemas, (group) =>
//                {
//                   // group.Execute();
//                });
//            }

//            AssetDatabase.SaveAssets();

//            EditorUtility.DisplayDialog("Complete", "Set Asset Complete", "OK");
//        }
//        [PropertyOrder(300)]
//        [Button("Clean Addressable", ButtonSizes.Large)]
//        public void CleanAddressable()
//        {
//            if (groupSchemas != null && groupSchemas.Length > 0)
//            {
//                Array.ForEach(groupSchemas, (group) =>
//                {
//                    //AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
//                    //AddressableAssetGroup assetGroup = settings.FindGroup(group.groupName);
//                    //if(assetGroup!=null)
//                    //{
//                    //    settings.RemoveGroup(assetGroup);
//                    //}
//                });
//            }

//            EditorUtility.DisplayDialog("Complete", "Clean Asset Complete", "OK");
//        }
//    }
//}
