using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DotEditor.Core.Misc
{
    public static class MeshCombine
    {
        public static void Combine(GameObject[] items,
            string meshSavedAssetDir,
            bool isAsChild,
            bool isRemovedAfter,
            bool isAddMeshCollider)
        {
            if (items == null || items.Length == 0)
                return;

            List<Material> materialList = new List<Material>();
            List<CombineInstance> combineInstanceList = new List<CombineInstance>();
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }
                MeshFilter[] meshFilters = item.GetComponentsInChildren<MeshFilter>();
                if (meshFilters != null && meshFilters.Length > 0)
                {
                    CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
                    MeshRenderer[] renderer = new MeshRenderer[meshFilters.Length];
                    for (int i = 0; i < meshFilters.Length; ++i)
                    {
                        MeshRenderer r = meshFilters[i].GetComponent<MeshRenderer>();
                        if (r != null && r.sharedMaterials != null && r.sharedMaterials.Length > 0)
                        {
                            foreach (var mat in r.sharedMaterials)
                            {
                                if (!materialList.Contains(mat))
                                {
                                    materialList.Add(mat);
                                }
                            }
                        }

                        combineInstances[i].mesh = meshFilters[i].sharedMesh;
                        combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    }

                    combineInstanceList.AddRange(combineInstances);
                }

            }

            if (combineInstanceList.Count == 0)
            {
                return;
            }

            string name = $"{items[0].name}_combine_mesh";
            string fileAssetPath = meshSavedAssetDir + "/" + name + ".mesh";
            GameObject combineGO = new GameObject(name);
            if (isAsChild)
            {
                if (items[0].transform.parent != null)
                {
                    combineGO.transform.SetParent(items[0].transform.parent.transform, true);
                }
            }
            if (materialList.Count > 0)
            {
                MeshRenderer combineRenderer = combineGO.AddComponent<MeshRenderer>();
                combineRenderer.sharedMaterials = materialList.ToArray();
            }
            MeshFilter combineMeshFilter = combineGO.AddComponent<MeshFilter>();
            combineMeshFilter.sharedMesh = new Mesh();
            combineMeshFilter.sharedMesh.CombineMeshes(combineInstanceList.ToArray());

            Mesh saveMesh = Object.Instantiate(combineMeshFilter.sharedMesh);
            AssetDatabase.CreateAsset(saveMesh, fileAssetPath);
            combineMeshFilter.sharedMesh = saveMesh;

            if (isRemovedAfter)
            {
                foreach (var delItem in items)
                {
                    GameObject.DestroyImmediate(delItem);
                }
            }

            if (isAddMeshCollider)
            {
                combineGO.AddComponent<MeshCollider>().sharedMesh = saveMesh;
            }

        }
    }
}
