using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    [CustomEditor(typeof(AssetGroupFilterOperation))]
    public class AssetGroupFilterOperationEditor : Editor
    {
        private SerializedProperty filterComposeType;
        private SerializedProperty assetFilters;

        private ReorderableList assetFilterRList;

        private SerializedProperty operationComposeType;
        private SerializedProperty assetOperations;

        private ReorderableList assetOperationRList;

        private void OnEnable()
        {
            filterComposeType = serializedObject.FindProperty("filterComposeType");
            assetFilters = serializedObject.FindProperty("assetFilters");

            operationComposeType = serializedObject.FindProperty("operationComposeType");
            assetOperations = serializedObject.FindProperty("assetOperations");

            assetFilterRList = new ReorderableList(serializedObject, assetFilters, true, true, true, true);
            assetFilterRList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Filter List");
            };
            assetFilterRList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, assetFilters.GetArrayElementAtIndex(index), new GUIContent("" + index));
                }
                EditorGUIUtil.EndLableWidth();
            };
            assetFilterRList.onAddCallback += (list) =>
            {
                assetFilters.InsertArrayElementAtIndex(assetFilters.arraySize);
            };

            assetOperationRList = new ReorderableList(serializedObject, assetOperations, true, true, true, true);
            assetOperationRList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Operation List");
            };
            assetOperationRList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, assetOperations.GetArrayElementAtIndex(index), new GUIContent("" + index));
                }
                EditorGUIUtil.EndLableWidth();
            };
            assetOperationRList.onAddCallback += (list) =>
            {
                assetOperations.InsertArrayElementAtIndex(assetOperations.arraySize);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(filterComposeType);
            assetFilterRList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(operationComposeType);
            assetOperationRList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
