using Dot.Core.World;
using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.World
{
    [CustomPropertyDrawer(typeof(StaticObjectLightmap))]
    public class StaticObjectLightmapPropertyDrawer : PropertyDrawer
    {
        private ReorderableList rendererLightmapRList = null;
        private SerializedProperty rendererLightmaps = null;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(rendererLightmapRList == null)
            {
                rendererLightmaps = property.FindPropertyRelative("rendererLightmaps");
                rendererLightmapRList = new ReorderableList(rendererLightmaps.serializedObject, rendererLightmaps, false, true, false, false);
                rendererLightmapRList.drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "Renderer Lightmap");
                };
                rendererLightmapRList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUIUtil.BeginLabelWidth(40);
                    {
                        EditorGUI.PropertyField(rect, rendererLightmaps.GetArrayElementAtIndex(index), new GUIContent("" + index));
                    }
                    EditorGUIUtil.EndLableWidth();
                };
                rendererLightmapRList.elementHeightCallback = (index) =>
                {
                    return StaticObjectRendererLightmapPropertyDrawer.PROPERTY_HEIGHT;
                };
            }
            rendererLightmapRList.DoList(position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight+10;
            if(rendererLightmaps == null || rendererLightmaps.arraySize == 0)
            {
                height += EditorGUIUtility.singleLineHeight * 2;
            }else
            {
                height += rendererLightmaps.arraySize * StaticObjectRendererLightmapPropertyDrawer.PROPERTY_HEIGHT;
            }

            return height;
        }
    }
}
