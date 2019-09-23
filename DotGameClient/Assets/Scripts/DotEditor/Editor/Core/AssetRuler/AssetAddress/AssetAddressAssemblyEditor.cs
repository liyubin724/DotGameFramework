using Dot.Core.Loader.Config;
using DotEditor.Core.Packer;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static DotEditor.Core.Packer.AssetBundleTagConfig;

namespace DotEditor.Core.AssetRuler.AssetAddress
{
    [CustomEditor(typeof(AssetAddressAssembly))]
    public class AssetAddressAssemblyEditor : AssetAssemblyEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawGroup();
            DrawOperation();

            if(GUILayout.Button("Execute",GUILayout.Height(40)))
            {
                CreateTagConfig((target as AssetAddressAssembly).Execute() as AssetAddressAssemblyResult);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateTagConfig(AssetAddressAssemblyResult result)
        {
            AssetBundleTagConfig tagConfig = BundlePackUtil.FindOrCreateTagConfig();
            tagConfig.groupDatas.Clear();

            foreach(var groupResult in result.groupResults)
            {
                AssetAddressGroupResult gResult = groupResult as AssetAddressGroupResult;

                AssetBundleGroupData groupData = new AssetBundleGroupData();
                groupData.groupName = gResult.groupName;
                groupData.isMain = gResult.isMain;
                groupData.isPreload = gResult.isPreload;

                tagConfig.groupDatas.Add(groupData);

                foreach(var operationResult in gResult.operationResults)
                {
                    AssetAddressOperationResult oResult = operationResult as AssetAddressOperationResult;
                    foreach(var kvp in oResult.addressDataDic)
                    {
                        AssetAddressData aaData = new AssetAddressData();
                        AssetAddressData kvpValue = kvp.Value as AssetAddressData;

                        aaData.assetAddress = kvpValue.assetAddress;
                        aaData.assetPath = kvpValue.assetPath;
                        aaData.bundlePath = kvpValue.bundlePath;
                        aaData.labels = new List<string>(kvpValue.labels).ToArray();

                        groupData.assetDatas.Add(aaData);
                    }
                }
            }


            EditorUtility.SetDirty(tagConfig);
            AssetDatabase.SaveAssets();
        }
    }
}
