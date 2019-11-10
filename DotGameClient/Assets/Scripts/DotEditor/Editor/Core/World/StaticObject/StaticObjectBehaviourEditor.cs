using Dot.Core.World;
using DotEditor.Core.EGUI;
using DotEditor.Core.Util;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DotEditor.Core.World.StaticObject
{
    [CustomEditor(typeof(StaticObjectBehaviour))]
    public class StaticObjectBehaviourEditor : Editor
    {
        private SerializedProperty meshRenderers;
        private SerializedProperty childBehaviours;

        private ReorderableList meshRenderersRList = null;
        private ReorderableList childBehavioursRList = null;

        private void OnEnable()
        {
            meshRenderers = serializedObject.FindProperty("meshRenderers");
            childBehaviours = serializedObject.FindProperty("childBehaviours");

            meshRenderersRList = new ReorderableList(serializedObject, meshRenderers, false, true, false, false);
            meshRenderersRList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "MeshRenderer");
            };
            meshRenderersRList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, meshRenderers.GetArrayElementAtIndex(index), new GUIContent("" + index));
                }
                EditorGUIUtil.EndLableWidth();
            };

            childBehavioursRList = new ReorderableList(serializedObject, childBehaviours, false, true, false, false);
            childBehavioursRList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Child Behaviour");
            };
            childBehavioursRList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUIUtil.BeginLabelWidth(40);
                {
                    EditorGUI.PropertyField(rect, childBehaviours.GetArrayElementAtIndex(index), new GUIContent("" + index));
                }
                EditorGUIUtil.EndLableWidth();
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            serializedObject.Update();
            meshRenderersRList.DoLayoutList();
            childBehavioursRList.DoLayoutList();

            EditorGUILayout.Space();

            if(GUILayout.Button("Find"))
            {
                meshRenderers.ClearArray();
                childBehaviours.ClearArray();

                List<MeshRenderer> renderers = new List<MeshRenderer>();
                List<StaticObjectBehaviour> child = new List<StaticObjectBehaviour>();

                StaticObjectBehaviour objectBehaviour = target as StaticObjectBehaviour;
                List<Transform> transforms = new List<Transform>();
                transforms.Add(objectBehaviour.transform);
                while(transforms.Count>0)
                {
                    Transform tran = transforms[0];
                    transforms.RemoveAt(0);

                    for(int i =0;i<tran.childCount;i++)
                    {
                        Transform childTran = tran.GetChild(i);
                        StaticObjectBehaviour childObjBeh = childTran.GetComponent<StaticObjectBehaviour>();
                        if(childObjBeh!=null)
                        {
                            child.Add(childObjBeh);
                            continue;
                        }
                        MeshRenderer meshRenderer = childTran.GetComponent<MeshRenderer>();
                        if(meshRenderer!=null)
                        {
                            renderers.Add(meshRenderer);
                        }

                        transforms.Add(childTran);
                    }
                }

                if(renderers.Count>0)
                {
                    SerializedPropertyUtil.SetArray<MeshRenderer>(meshRenderers, renderers);
                }
                if(child.Count>0)
                {
                    SerializedPropertyUtil.SetArray<StaticObjectBehaviour>(childBehaviours, child);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
