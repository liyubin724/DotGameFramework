using DotEditor.Core.Util;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetAssembly : ScriptableObject
    {
        public AssetAssemblyType assetAssemblyType = AssetAssemblyType.AssetAddress;
        public List<AssetGroup> assetGroups = new List<AssetGroup>();

        public virtual AssetAssemblyResult Execute()
        {
            return null;
        }

        public void AutoFind()
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssets<AssetGroup>();
            List<AssetGroup> groupList = new List<AssetGroup>();
            foreach (var assetPath in assetPaths)
            {
                AssetGroup group = AssetDatabase.LoadAssetAtPath<AssetGroup>(assetPath);
                if (group != null && group.assetAssemblyType == assetAssemblyType)
                {
                    groupList.Add(group);
                }
            }
            assetGroups.AddRange(groupList);

            EditorUtility.SetDirty(this);
        }
    }
}
