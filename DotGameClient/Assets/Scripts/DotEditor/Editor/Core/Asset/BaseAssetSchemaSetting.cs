using DotEditor.Core.Util;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseAssetSchemaSetting : ScriptableObject
    {
        [PropertyOrder(100)]
        public string settingName = "Asset Schema Setting";

        [PropertyOrder(101)]
        [EnumToggleButtons]
        public AssetGroupType groupType = AssetGroupType.Addressable;

        [PropertyOrder(102)]
        [ListDrawerSettings(Expanded =true,IsReadOnly =true)]
        public AssetGroupSchema[] groupSchemas = new AssetGroupSchema[0];

        [PropertyOrder(200)]
        [Button("Auto Find Group",ButtonSizes.Large)]
        public void AutoFindGroup()
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssets<AssetGroupSchema>();
            List<AssetGroupSchema> groupList = new List<AssetGroupSchema>();
            foreach(var assetPath in assetPaths)
            {
                AssetGroupSchema group = AssetDatabase.LoadAssetAtPath<AssetGroupSchema>(assetPath);
                if(group!=null && group.groupType == groupType)
                {
                    groupList.Add(group);
                }
            }
            groupSchemas = groupList.ToArray();
        }
    }
}
