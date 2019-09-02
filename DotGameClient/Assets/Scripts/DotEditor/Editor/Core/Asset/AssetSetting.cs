using DotEditor.Core.Util;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.AddressableAssets;
//using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CreateAssetMenu(fileName = "asset_setting", menuName = "Asset/Asset Setting")]
    public class AssetSetting : ScriptableObject
    {
        [ListDrawerSettings(Expanded =true,IsReadOnly =true)]
        public AssetGroupSchema[] addressableGroups = new AssetGroupSchema[0];

        [Button("Auto Find Asset Group",ButtonSizes.Large)]
        public void FindAssetGroup()
        {
            string[] assets = AssetDatabaseUtil.FindAssets<AssetGroupSchema>();
            List<AssetGroupSchema> addressableGroupList = new List<AssetGroupSchema>();
            for(int i =0;i<assets.Length;i++)
            {
                AssetGroupSchema group = AssetDatabase.LoadAssetAtPath<AssetGroupSchema>(assets[i]);
                if(group.groupType == AssetGroupType.Addressable)
                {
                    addressableGroupList.Add(group);
                }
            }
            addressableGroups = addressableGroupList.ToArray();
        }

        [Button("Execute Addressable", ButtonSizes.Large)]
        public void ExecuteAddressable()
        {
            if(addressableGroups!=null && addressableGroups.Length>0)
            {
                Array.ForEach(addressableGroups, (group) =>
                {
                    group.Execute();
                });
            }

            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Complete", "Set Asset Complete", "OK");
        }

        [Button("Clean Addressable", ButtonSizes.Large)]
        public void CleanAddressable()
        {
            if (addressableGroups != null && addressableGroups.Length > 0)
            {
                Array.ForEach(addressableGroups, (group) =>
                {
                    //AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                    //AddressableAssetGroup assetGroup = settings.FindGroup(group.groupName);
                    //if(assetGroup!=null)
                    //{
                    //    settings.RemoveGroup(assetGroup);
                    //}
                });
            }

            EditorUtility.DisplayDialog("Complete", "Clean Asset Complete", "OK");
        }
    }
}
