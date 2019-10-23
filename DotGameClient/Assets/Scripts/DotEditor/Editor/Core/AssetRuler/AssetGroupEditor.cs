using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetGroupEditor : Editor
    {
        SerializedProperty isEnable;
        SerializedProperty groupName;
        SerializedProperty assetAssemblyType;
        SerializedProperty assetSearchers;
        SerializedProperty filterOperations;

        private ReorderableList filterOperationRList;
        private ReorderableList assetSearcherRList;
        protected virtual void OnEnable()
        {
            isEnable = serializedObject.FindProperty("isEnable");
            groupName = serializedObject.FindProperty("groupName");
            assetAssemblyType = serializedObject.FindProperty("assetAssemblyType");
            assetSearchers = serializedObject.FindProperty("assetSearchers");
            filterOperations = serializedObject.FindProperty("filterOperations");

            assetSearcherRList = new ReorderableList(serializedObject, assetSearchers, false, true, true, true);
            assetSearcherRList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Asset Searcher List");
            };
            assetSearcherRList.drawElementCallback = (curRect, index, isActive, isFocused) =>
            {
                SerializedProperty property = assetSearchers.GetArrayElementAtIndex(index);

                SerializedProperty folder = property.FindPropertyRelative("folder");
                curRect.height = EditorGUIUtility.singleLineHeight;
                folder.stringValue = EditorGUIUtil.DrawAssetFolderSelection(curRect, "Folder", folder.stringValue);

                SerializedProperty includeSubfolder = property.FindPropertyRelative("includeSubfolder");
                curRect.y += curRect.height;
                EditorGUI.PropertyField(curRect, includeSubfolder);

                SerializedProperty fileNameFilterRegex = property.FindPropertyRelative("fileNameFilterRegex");
                curRect.y += curRect.height;
                EditorGUI.PropertyField(curRect, fileNameFilterRegex);
            };
            assetSearcherRList.elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight * 3 + 4;
            };

            filterOperationRList = new ReorderableList(serializedObject,filterOperations, true, true, true, true);
            filterOperationRList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Filter Operation List");
            };
            filterOperationRList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, filterOperations.GetArrayElementAtIndex(index),new GUIContent(""+index));  
                }
                EditorGUIUtil.EndLableWidth();
            };
            filterOperationRList.onAddCallback += (list) =>
            {
                filterOperations.InsertArrayElementAtIndex(filterOperations.arraySize);
            };
        }

        protected void DrawBaseInfo()
        {
            EditorGUILayout.PropertyField(isEnable);
            EditorGUILayout.PropertyField(groupName);
            EditorGUILayout.PropertyField(assetAssemblyType);
        }

        protected void DrawAssetSearcher()
        {
            assetSearcherRList.DoLayoutList();
        }

        protected void DrawFilterOperations()
        {
            filterOperationRList.DoLayoutList();
        }
    }
}
