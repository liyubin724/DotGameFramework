using Dot.Core.World;
using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.World
{
    [CustomEditor(typeof(WorldStaticObjectBehaviar))]
    public class WorldObjectBehaviarEditor : Editor
    {
        private SerializedProperty renderers;
        private SerializedProperty bounds;

        private ReorderableList renderersRList;

        protected void OnEnable()
        {
            renderers = serializedObject.FindProperty("renderers");
            bounds = serializedObject.FindProperty("bounds");

            renderersRList = new ReorderableList(serializedObject, renderers, false, true, false, false);
            renderersRList.drawHeaderCallback = (rect) =>
             {
                 EditorGUI.LabelField(rect, "Renderers List:");
             };
            renderersRList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, renderers.GetArrayElementAtIndex(index), new GUIContent("" + index));
                }
                EditorGUIUtil.EndLableWidth();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawRenderers();
            DrawBounds();

            serializedObject.ApplyModifiedProperties();
        }
        
        protected void DrawRenderers()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField(new GUIContent("Renderers:"), EditorGUIStyle.BoldLabelStyle, GUILayout.Height(20));
                EditorGUILayout.HelpBox("if the filed(renderers) changed please generate lighting the scene which used the prefab", MessageType.Warning);
                renderersRList.DoLayoutList();
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Find"))
                    {
                        AutoFindRenderer();
                    }
                    if (GUILayout.Button("Clear"))
                    {
                        renderers.ClearArray();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void AutoFindRenderer()
        {
            WorldStaticObjectBehaviar beh = target as WorldStaticObjectBehaviar;
            renderers.ClearArray();

            Renderer[] r = beh.gameObject.GetComponentsInChildren<MeshRenderer>();
            Debug.Log(r.Length);
            if (r != null && r.Length > 0)
            {
                renderers.arraySize = r.Length;

                for (int i = 0; i < r.Length; i++)
                {
                    renderers.GetArrayElementAtIndex(i).objectReferenceValue = r[i];
                }
            }
        }

        private void DrawBounds()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField(new GUIContent("Bounds:"), EditorGUIStyle.BoldLabelStyle, GUILayout.Height(20));
                EditorGUI.BeginDisabledGroup(true);
                {
                    EditorGUILayout.PropertyField(bounds);
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Recalculate "))
                {
                    WorldStaticObjectBehaviar beh = target as WorldStaticObjectBehaviar;
                    Renderer[] r = beh.gameObject.GetComponentsInChildren<MeshRenderer>();
                    Bounds b = new Bounds();
                    if (r != null && r.Length > 0)
                    {
                        for (int i = 0; i < r.Length; i++)
                        {
                            b.Encapsulate(r[i].bounds);
                        }
                    }
                    bounds.boundsValue = b;
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
