using Dot.Core.World;
using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.World
{
    [CustomPropertyDrawer(typeof(StaticObjectRendererLightmap))]
    public class StaticObjectRendererLightmapPropertyDrawer : PropertyDrawer
    {
        public static float PROPERTY_HEIGHT = EditorGUIUtility.singleLineHeight * 3;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty rendererIndex = property.FindPropertyRelative("rendererIndex");
            SerializedProperty lightmapIndex = property.FindPropertyRelative("lightmapIndex");
            SerializedProperty lightmapOffset = property.FindPropertyRelative("lightmapOffset");

            EditorGUI.BeginDisabledGroup(true);
            {
                Rect rect = position;
                EditorGUIUtil.BeginLabelWidth(100);
                {
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, rendererIndex);

                    rect.y += rect.height;
                    EditorGUI.PropertyField(rect, lightmapIndex);
                }
                EditorGUIUtil.EndLableWidth();
                rect.y += rect.height;
                rect.width = 100;
                EditorGUI.LabelField(rect, "lightmap Offset");
                rect.x += rect.width;
                rect.width = (position.width - 100) * 0.25f;
                EditorGUIUtil.BeginLabelWidth(25);
                {
                    EditorGUI.PropertyField(rect, lightmapOffset.FindPropertyRelative("x"));
                    rect.x += rect.width;
                    EditorGUI.PropertyField(rect, lightmapOffset.FindPropertyRelative("y"));
                    rect.x += rect.width;
                    EditorGUI.PropertyField(rect, lightmapOffset.FindPropertyRelative("z"));
                    rect.x += rect.width;
                    EditorGUI.PropertyField(rect, lightmapOffset.FindPropertyRelative("w"));
                }
                EditorGUIUtil.EndLableWidth();
                
            }
            EditorGUI.EndDisabledGroup();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return PROPERTY_HEIGHT;
        }
    }
}
