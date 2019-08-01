using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Editor.Core.Misc
{
    public class MeshCombineWindow : OdinEditorWindow
    {
        [MenuItem("Game/Tools/Misc/Combine Mesh Window")]
        private static void ShowWindow()
        {
            var win = GetWindow<MeshCombineWindow>();
            win.titleContent = new GUIContent("Mesh Combine");
        }

        [FilePath(AbsolutePath =true)]
        public string meshSavedAssetDir = "Assets";
         
        public bool isAsChild = true;
        public bool isRemovedAfter = true;
        public bool isAddMeshCollider = false;

        [Button("Combine Children",ButtonSizes.Large)]
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

            MeshCombine.Combine(new GameObject[] { Selection.activeGameObject }, meshSavedAssetDir, isAsChild, isRemovedAfter, isAddMeshCollider);
        }

        [Button("Combine All Selected", ButtonSizes.Large)]
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
            
            MeshCombine.Combine(gObjs, meshSavedAssetDir, isAsChild, isRemovedAfter, isAddMeshCollider);
        }
    }
}
