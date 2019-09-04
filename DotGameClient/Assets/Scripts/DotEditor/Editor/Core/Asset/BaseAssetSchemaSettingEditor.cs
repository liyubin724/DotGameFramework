using DotEditor.Core.EGUI;
using DotEditor.Core.Util;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    public class BaseAssetSchemaSettingEditor :Editor
    {
        private SerializedProperty settingName;
        private SerializedProperty groupType;
        private SerializedProperty groupSchemas;

        private ReorderableList groupList = null;
        protected virtual void OnEnable()
        {
            settingName = serializedObject.FindProperty("settingName");
            groupType = serializedObject.FindProperty("groupType");
            groupSchemas = serializedObject.FindProperty("groupSchemas");

            groupList = new ReorderableList(serializedObject, groupSchemas, false, true, false, false);

            groupList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "Group List");
            };
            groupList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        SerializedProperty serializedProperty = groupSchemas.GetArrayElementAtIndex(index);
                        EditorGUI.PropertyField(rect, serializedProperty, new GUIContent("" + index));
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUIUtil.EndLableWidth();
            };
        }

        protected void DrawSetting()
        {
            EditorGUILayout.PropertyField(settingName);
            EditorGUILayout.PropertyField(groupType);
        }

        protected void DrawGroups()
        {
            groupList.DoLayoutList();
        }

        protected void DrawOperation()
        {
            if(GUILayout.Button("Auto Find Group",GUILayout.Height(40)))
            {
                AutoFindGroup();
            }
        }

        public void AutoFindGroup()
        {
            string[] assetPaths = AssetDatabaseUtil.FindAssets<BaseGroupSchema>();
            List<BaseGroupSchema> groupList = new List<BaseGroupSchema>();
            foreach (var assetPath in assetPaths)
            {
                BaseGroupSchema group = AssetDatabase.LoadAssetAtPath<BaseGroupSchema>(assetPath);
                if (group != null && group.groupType == (AssetGroupType)groupType.intValue)
                {
                    groupList.Add(group);
                }
            }
            groupSchemas.ClearArray();
            for(int i =0;i<groupList.Count;++i)
            {
                groupSchemas.InsertArrayElementAtIndex(i);
                groupSchemas.GetArrayElementAtIndex(i).objectReferenceValue = groupList[i];
            }
        }

    }
}
