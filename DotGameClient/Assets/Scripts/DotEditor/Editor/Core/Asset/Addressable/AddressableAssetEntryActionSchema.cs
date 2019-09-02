﻿using Dot.Core.Util;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
//using UnityEditor.AddressableAssets;
//using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    

    [CreateAssetMenu(fileName = "addressable_entry_scheme", menuName = "Asset/Action/Addressable/Entry Action Scheme")]
    public class AddressableAssetEntryActionSchema : BaseActionSchema
    {
        public AssetAddressMode addressType = AssetAddressMode.FullPath;
        [ShowIf("IsShowFileNameFormat")]
        public string fileNameFormat = "{0}";
        public string filterFolder = "";
        public string[] labels = new string[0];

        public void Execute(AssetGroupActionData actionData)
        {
            if (!isEnable) return;

            //AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            //AddressableAssetGroup assetGroup = settings.FindGroup(actionData.groupName);
            //if(assetGroup == null)
            //{
            //    Debug.LogError("AddressableAssetEntryActionSchema::Execute->Group Not Found");
            //    return;
            //}
            //if (actionData.filterDatas != null && actionData.filterDatas.Count > 0)
            //{
                
            //    foreach (var filterData in actionData.filterDatas)
            //    {
            //        if (!string.IsNullOrEmpty(filterFolder) && filterFolder != filterData.filterFolder)
            //        {
            //            continue;
            //        }
            //        foreach (var asset in filterData.assets)
            //        {
            //            if (asset != null)
            //            {
            //                string guid = AssetDatabase.AssetPathToGUID(asset);
            //                AddressableAssetEntry assetEntry = assetGroup.GetAssetEntry(guid);
            //                if (assetEntry == null)
            //                {
            //                    assetEntry = settings.CreateOrMoveEntry(guid, assetGroup,false,false);
            //                }
            //                assetEntry.SetAddress(GetAssetAddress(asset),false);
            //                if(labels!=null && labels.Length>0)
            //                {
            //                    foreach(var label in labels)
            //                    {
            //                        assetEntry.SetLabel(label, true,true,false);
            //                    }
            //                }
            //            }
            //        }
            //    }

                
            //}
        }

        private string GetAssetAddress(string assetPath)
        {
            if (addressType == AssetAddressMode.FullPath)
                return assetPath;
            else if (addressType == AssetAddressMode.FileName)
                return Path.GetFileName(assetPath);
            else if (addressType == AssetAddressMode.FileNameWithoutExtension)
                return Path.GetFileNameWithoutExtension(assetPath);
            else if (addressType == AssetAddressMode.FileFormatName)
                return string.Format(fileNameFormat, Path.GetFileNameWithoutExtension(assetPath));
            else
                return assetPath;
        }

        #region odin
        private bool IsShowFileNameFormat() => addressType == AssetAddressMode.FileFormatName;

        #endregion
    }
}
