using DotEditor.Core.EGUI;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(AssetFilterSchema))]
    public class AssetFilterSchemaEditor : Editor
    {
        private SerializedProperty isEnable;
        private SerializedProperty folder;
        private SerializedProperty includeSubfolder;
        private SerializedProperty fileNameFilterRegex;
        private SerializedProperty assets;

        private ReorderableList assetList = null;
        private void OnEnable()
        {
            isEnable = serializedObject.FindProperty("isEnable");
            folder = serializedObject.FindProperty("folder");
            includeSubfolder = serializedObject.FindProperty("includeSubfolder");
            fileNameFilterRegex = serializedObject.FindProperty("fileNameFilterRegex");
            assets = serializedObject.FindProperty("assets");

            assetList = new ReorderableList(serializedObject, assets, false, true, false, false);
            
            assetList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "Filter Asset List");
            };
            assetList.drawElementCallback += ( rect,  index,  isActive,  isFocused) =>
            {
                EditorGUIUtil.BeginSetLabelWidth(40);
                {
                    SerializedProperty serializedProperty = assets.GetArrayElementAtIndex(index);
                    EditorGUI.LabelField(rect, "" + index, serializedProperty.stringValue, EditorStyles.textField);
                }
                EditorGUIUtil.EndSetLableWidth();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(isEnable);
            EditorGUILayoutUtil.DrawFolderSelection(folder);
            EditorGUILayout.PropertyField(includeSubfolder);
            EditorGUILayout.PropertyField(fileNameFilterRegex);

            EditorGUILayout.Space();
 
            
            if (GUILayout.Button("Execute",GUILayout.Height(40)))
            {
                Array.ForEach(targets, (target) =>
                {
                    (target as AssetFilterSchema).Execute();
                });
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("为了方便查看，临时保存着筛选后的结果。为了保证正确，请在使用前点击下方按钮或者调用Execute方法重新筛选", MessageType.Warning);

            assetList.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
