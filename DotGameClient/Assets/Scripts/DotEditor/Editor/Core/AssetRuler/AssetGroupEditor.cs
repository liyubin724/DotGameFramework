using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static DotEditor.Core.AssetRuler.AssetGroup;

namespace DotEditor.Core.AssetRuler
{
    public class AssetGroupEditor : Editor
    {
        SerializedProperty isEnable;
        SerializedProperty groupName;
        SerializedProperty assetAssemblyType;
        SerializedProperty assetSearcher;
        SerializedProperty filterOperations;

        private ReorderableList filterOperationRList;
        protected virtual void OnEnable()
        {
            isEnable = serializedObject.FindProperty("isEnable");
            groupName = serializedObject.FindProperty("groupName");
            assetAssemblyType = serializedObject.FindProperty("assetAssemblyType");
            assetSearcher = serializedObject.FindProperty("assetSearcher");
            filterOperations = serializedObject.FindProperty("filterOperations");

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
            EditorGUILayout.PropertyField(assetSearcher);
        }

        protected void DrawFilterOperations()
        {
            filterOperationRList.DoLayoutList();
        }
    }

    [CustomPropertyDrawer(typeof(AssetSearcher))]
    public class AssetSearcherPropertyDrawer : PropertyDrawer
    {
        private bool isFoldout = true;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect curRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            isFoldout = EditorGUI.Foldout(curRect, isFoldout, label);
            if (isFoldout)
            {
                SerializedProperty folder = property.FindPropertyRelative("folder");
                curRect.y += curRect.height;
                curRect.x += 20;
                curRect.width -= 20;
                folder.stringValue = EditorGUIUtil.DrawAssetFolderSelection(curRect,"Folder", folder.stringValue);

                SerializedProperty includeSubfolder = property.FindPropertyRelative("includeSubfolder");
                curRect.y += curRect.height;
                EditorGUI.PropertyField(curRect, includeSubfolder);

                SerializedProperty fileNameFilterRegex = property.FindPropertyRelative("fileNameFilterRegex");
                curRect.y += curRect.height;
                EditorGUI.PropertyField(curRect, fileNameFilterRegex);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (isFoldout)
            {
                height += EditorGUIUtility.singleLineHeight * 3;
            }

            return height;
        }
    }
}
