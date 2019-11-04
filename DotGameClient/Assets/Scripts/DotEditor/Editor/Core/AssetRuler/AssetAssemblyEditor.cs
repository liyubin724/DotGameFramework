using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.AssetRuler
{
    public class AssetAssemblyEditor : Editor
    {
        private SerializedProperty assetAssemblyType = null;
        private SerializedProperty assetGroups = null;

        private ReorderableList groupList = null;
        protected virtual void OnEnable()
        {
            assetAssemblyType = serializedObject.FindProperty("assetAssemblyType");
            assetGroups = serializedObject.FindProperty("assetGroups");

            groupList = new ReorderableList(serializedObject, assetGroups, false, true, false, false);
            groupList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Asset Group List:");
            };
            groupList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, assetGroups.GetArrayElementAtIndex(index), new GUIContent("" + index));
                }
                EditorGUIUtil.EndLableWidth();
            };
        }

        protected override bool ShouldHideOpenButton()
        {
            return true;
        }

        protected void DrawGroup()
        {
            EditorGUILayout.PropertyField(assetAssemblyType);
            groupList.DoLayoutList();
        }

        protected void DrawOperation()
        {
            if (GUILayout.Button("Auto Find Group", GUILayout.Height(40)))
            {
                (target as AssetAssembly).AutoFind();
            }
        }
    }
}
