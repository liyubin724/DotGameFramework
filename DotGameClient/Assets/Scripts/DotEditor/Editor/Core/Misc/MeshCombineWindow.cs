using DotEditor.Core.EGUI;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Misc
{
    public class MeshCombineWindow : EditorWindow
    {
        [MenuItem("Game/Tools/Misc/Combine Mesh Window")]
        private static void ShowWindow()
        {
            var win = GetWindow<MeshCombineWindow>();
            win.titleContent = new GUIContent("Mesh Combine");
            win.Show();
        }

        public string meshSavedAssetDir = "Assets";
        public bool isCombineAsChild = true;
        public bool isDeleteAfterCombined = true;
        public bool isAddMeshCollider = false;

        private GUIStyle centerLabelTitleStyle = null;
        private void OnEnable()
        {
            centerLabelTitleStyle = new GUIStyle(EditorStyles.label);
            centerLabelTitleStyle.alignment = TextAnchor.MiddleCenter;
            centerLabelTitleStyle.fontSize = 40;
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("Mesh Combine Tool", centerLabelTitleStyle, GUILayout.ExpandWidth(true));
            meshSavedAssetDir = EditorGUILayoutUtil.DrawAssetFolderSelection("Saved Dir", meshSavedAssetDir, true);
            isCombineAsChild = EditorGUILayout.Toggle("Combine As Child", isCombineAsChild);
            isDeleteAfterCombined = EditorGUILayout.Toggle("Delete After Combined", isDeleteAfterCombined);
            isAddMeshCollider = EditorGUILayout.Toggle("Add Mesh Collider", isAddMeshCollider);

            if(GUILayout.Button("Combine Selected",GUILayout.Height(40),GUILayout.ExpandWidth(true)))
            {
                CombineSelected();
            }
            if(GUILayout.Button("Combine Multiple Selected", GUILayout.Height(40), GUILayout.ExpandWidth(true)))
            {
                CombineMultipleSeleted();
            }
        }

        private void CombineSelected()
        {
            if (Selection.activeGameObject == null)
            {
                EditorUtility.DisplayDialog("Warning", "Please selected a GameObject which you want to combine", "OK");

                return;
            }
            if (string.IsNullOrEmpty(meshSavedAssetDir))
            {
                EditorUtility.DisplayDialog("Warning", "Please selected a directory which the mesh will be saved", "OK");

                return;
            }

            MeshCombine.Combine(new GameObject[] { Selection.activeGameObject }, meshSavedAssetDir, isCombineAsChild, isDeleteAfterCombined, isAddMeshCollider);
        }

        private void CombineMultipleSeleted()
        {
            GameObject[] gObjs = Selection.gameObjects;
            if (gObjs == null || gObjs.Length ==0)
            {
                EditorUtility.DisplayDialog("Warning", "Please selected a GameObject which you want to combine", "OK");

                return;
            }
            if (string.IsNullOrEmpty(meshSavedAssetDir))
            {
                EditorUtility.DisplayDialog("Warning", "Please selected a directory which the mesh will be saved", "OK");

                return;
            }
            
            MeshCombine.Combine(gObjs, meshSavedAssetDir, isCombineAsChild, isDeleteAfterCombined, isAddMeshCollider);
        }
    }
}
