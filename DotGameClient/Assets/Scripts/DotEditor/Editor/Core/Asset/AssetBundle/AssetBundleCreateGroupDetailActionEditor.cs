using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.Asset
{
    [CustomEditor(typeof(AssetBundleCreateGroupDetailAction))]
    public class CreateAssetGroupDataActionEditor : BaseActionSchemaEditor
    {
        private SerializedProperty packMode;
        private SerializedProperty packCount;
        private SerializedProperty addressMode;
        private SerializedProperty fileNameFormat;
        private SerializedProperty filterFolder;
        private SerializedProperty labels;


        private ReorderableList labelList = null;
        protected override void OnEnable()
        {
            base.OnEnable();
            packMode = serializedObject.FindProperty("packMode");
            packCount = serializedObject.FindProperty("packCount");
            addressMode = serializedObject.FindProperty("addressMode");
            fileNameFormat = serializedObject.FindProperty("fileNameFormat");
            filterFolder = serializedObject.FindProperty("filterFolder");
            labels = serializedObject.FindProperty("labels");

            labelList = new ReorderableList(serializedObject, labels, true, true, true, true);

            labelList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(rect, "Label List");
            };
            labelList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginSetLabelWidth(40);
                {
                    SerializedProperty serializedProperty = labels.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, serializedProperty, new GUIContent("" + index));
                }
                EditorGUIUtil.EndSetLableWidth();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawIsEnable();
            DrawActionName();

            EditorGUILayout.PropertyField(packMode);
            if((AssetBundlePackMode)packMode.intValue == AssetBundlePackMode.GroupByCount)
            {
                EditorGUILayout.PropertyField(packCount);
            }else
            {
                packCount.intValue = 0;
            }
            EditorGUILayout.PropertyField(addressMode);
            if((AssetAddressMode)addressMode.intValue == AssetAddressMode.FileFormatName)
            {
                EditorGUILayout.PropertyField(fileNameFormat);
            }
            EditorGUILayoutUtil.DrawFolderSelection(filterFolder);

            labelList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

    }
}
