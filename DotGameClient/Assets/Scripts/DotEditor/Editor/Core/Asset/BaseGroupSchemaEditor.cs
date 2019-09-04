using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseGroupSchemaEditor : Editor
    {
        private SerializedProperty isEnable;
        private SerializedProperty groupName;
        private SerializedProperty groupType;
        private SerializedProperty filters;
        private SerializedProperty actions;

        private ReorderableList filterList = null;
        private ReorderableList actionList = null;
        protected virtual void OnEnable()
        {
            isEnable = serializedObject.FindProperty("isEnable");
            groupName = serializedObject.FindProperty("groupName");
            groupType = serializedObject.FindProperty("groupType");
            filters = serializedObject.FindProperty("filters");
            actions = serializedObject.FindProperty("actions");

            filterList = new ReorderableList(serializedObject, filters, true, true, true, true);
            filterList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "Filter Schema List:");
            };
            filterList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, filters.GetArrayElementAtIndex(index), new GUIContent("" + index));
                }
                EditorGUIUtil.EndLableWidth();
            };
            filterList.onAddCallback += (list) =>
            {
                filters.InsertArrayElementAtIndex(filters.arraySize);
            };

            actionList = new ReorderableList(serializedObject, actions, true, true, true, true);
            actionList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "Action Schema List:");
            };
            actionList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, actions.GetArrayElementAtIndex(index),new GUIContent(""+index));
                }
                EditorGUIUtil.EndLableWidth();
            };
            actionList.onAddCallback += (list) =>
            {
                actions.InsertArrayElementAtIndex(actions.arraySize);
            };
        }

        protected void DrawIsEnable()
        {
            EditorGUILayout.PropertyField(isEnable);
        }

        protected void DrawGroupInfo()
        {
            EditorGUILayout.PropertyField(groupName);
            EditorGUILayout.PropertyField(groupType);
        }

        protected void DrawFilters()
        {
            filterList.DoLayoutList();
        }

        protected void DrawActions()
        {
            actionList.DoLayoutList();
        }
    }
}
