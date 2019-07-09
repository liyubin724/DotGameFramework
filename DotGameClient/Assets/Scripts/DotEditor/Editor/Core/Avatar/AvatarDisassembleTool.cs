using Dot.Core.Avatar;
using Dot.Core.Entity;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace DotEditor.Core.Avatar
{
    public static class AvatarDisassembleTool
    {
        [MenuItem("Game/Avatar/1. Create Skeleton",false,1)]
        public static void CreateSkeleton()
        {
            UnityObject selObj = Selection.activeObject;
            if (selObj == null)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个需要拆分的模型,格式为FBX", "OK");
                return;
            }
            string fbxAssetPath = AssetDatabase.GetAssetPath(selObj.GetInstanceID());
            if (Path.GetExtension(fbxAssetPath).ToLower() != ".fbx")
            {
                EditorUtility.DisplayDialog("提示", "请选择一个需要拆分的模型,格式为FBX", "OK");
                return;
            }

            CreateSkeleton(fbxAssetPath);
        }

        [MenuItem("Game/Avatar/2. Create Renderer",false,2)]
        public static void CreateMeshRenderer()
        {
            UnityObject selObj = Selection.activeObject;
            if (selObj == null)
            {
                EditorUtility.DisplayDialog("提示", "请选择一个需要拆分的模型,格式为FBX", "OK");
                return;
            }
            string fbxAssetPath = AssetDatabase.GetAssetPath(selObj.GetInstanceID());
            if (Path.GetExtension(fbxAssetPath).ToLower() != ".fbx")
            {
                EditorUtility.DisplayDialog("提示", "请选择一个需要拆分的模型,格式为FBX", "OK");
                return;
            }

            CreateMeshRenderer(fbxAssetPath);
        }

        [MenuItem("Game/Avatar/3. Create Part",false,3)]
        public static void CreatePart()
        {
            UnityObject selObj = Selection.activeObject;
            if (selObj == null)
            {
                return;
            }

            AvatarRendererPart[] rParts = Selection.GetFiltered<AvatarRendererPart>(SelectionMode.Assets);
            if(rParts == null || rParts.Length == 0)
            {
                string selAssetPath = AssetDatabase.GetAssetPath(selObj.GetInstanceID());
                CreatePart(Path.GetDirectoryName(selAssetPath).Replace("\\", "/"));
            }else
            {
                string selAssetPath = AssetDatabase.GetAssetPath(rParts[0].GetInstanceID());
                CreatePart(Path.GetDirectoryName(selAssetPath).Replace("\\", "/"),rParts);
            }
        }

        public static void CreateSkeleton(string fbxAssetPath,string targetAssetDir=null)
        {
            if(string.IsNullOrEmpty(targetAssetDir))
            {
                targetAssetDir = Path.GetDirectoryName(fbxAssetPath).Replace("\\","/");
            }
            string skeletonPrefabAssetPath = string.Format("{0}/{1}_skeleton.prefab", targetAssetDir, Path.GetFileNameWithoutExtension(fbxAssetPath));

            UnityObject fbxObject = AssetDatabase.LoadAssetAtPath(fbxAssetPath,typeof(GameObject));
            GameObject instanceGO = GameObject.Instantiate<GameObject>(fbxObject as GameObject);

            EntityNodeBehaviour foNode = instanceGO.AddComponent<EntityNodeBehaviour>();

            List<EntityBoneNodeData> boneNodeList = new List<EntityBoneNodeData>();
            List<EntityMeshRendererNodeData> rendererNodeList = new List<EntityMeshRendererNodeData>();

            Transform[] transforms = instanceGO.GetComponentsInChildren<Transform>(true);
            foreach(var t in transforms)
            {
                if(t == instanceGO.transform)
                {
                    EntityBoneNodeData boneNode = new EntityBoneNodeData();
                    boneNodeList.Add(boneNode);
                    boneNode.name = "Root";
                    boneNode.transform = t;
                    continue;
                }
                SkinnedMeshRenderer smr = t.GetComponent<SkinnedMeshRenderer>();
                if(smr == null)
                {
                    EntityBoneNodeData boneNode = new EntityBoneNodeData();
                    boneNodeList.Add(boneNode);
                    boneNode.name = t.name;
                    boneNode.transform = t;
                }else
                {
                    EntityMeshRendererNodeData rendererNode = new EntityMeshRendererNodeData();
                    rendererNodeList.Add(rendererNode);
                    rendererNode.name = t.name;
                    rendererNode.renderer = smr;

                    smr.sharedMaterials = new Material[0];
                    smr.rootBone = null;
                    smr.sharedMesh = null;
                    smr.bones = new Transform[0];
                }
            }
            foNode.boneNodes = boneNodeList.ToArray();
            foNode.rendererNodes = rendererNodeList.ToArray();

            GameObject savedGO = AssetDatabase.LoadAssetAtPath<GameObject>(skeletonPrefabAssetPath);
            if(savedGO!=null)
            {
                EntityNodeBehaviour savedFONode = savedGO.GetComponent<EntityNodeBehaviour>();
                if(savedFONode == null)
                {
                    savedGO = null;
                    AssetDatabase.DeleteAsset(skeletonPrefabAssetPath);
                }else
                {
                    List<EntityBindNodeData> bindNodeList = new List<EntityBindNodeData>();
                    foreach(var bindNode in savedFONode.bindNodes)
                    {
                        if(bindNode.transform!=null)
                        {
                            EntityBoneNodeData boneNode = foNode.GetBoneNode(bindNode.transform.name);
                            if(boneNode != null)
                            {
                                EntityBindNodeData node = new EntityBindNodeData();
                                node.atlasName = bindNode.atlasName;
                                node.transform = boneNode.transform;
                                node.postionOffset = bindNode.postionOffset;
                                node.rotationOffset = bindNode.rotationOffset;
                                bindNodeList.Add(node);
                            }else if(bindNode.transform == savedGO.transform)
                            {
                                EntityBindNodeData node = new EntityBindNodeData();
                                node.atlasName = bindNode.atlasName;
                                node.transform = foNode.transform;
                                node.postionOffset = bindNode.postionOffset;
                                node.rotationOffset = bindNode.rotationOffset;
                                bindNodeList.Add(node);
                            }
                        }
                    }
                    foNode.bindNodes = bindNodeList.ToArray();
                }
            }

            PrefabUtility.SaveAsPrefabAsset(instanceGO, skeletonPrefabAssetPath);
            GameObject.DestroyImmediate(instanceGO);
        }

        public static void CreateMeshRenderer(string fbxAssetPath,string targetAssetDir = null)
        {
            if (string.IsNullOrEmpty(targetAssetDir))
            {
                targetAssetDir = Path.GetDirectoryName(fbxAssetPath).Replace("\\", "/");
            }

            UnityObject fbxObject = AssetDatabase.LoadAssetAtPath(fbxAssetPath, typeof(GameObject));
            ModelImporter modelImporter = (ModelImporter)AssetImporter.GetAtPath(fbxAssetPath);
            ModelImporterMeshCompression meshCompression = modelImporter.meshCompression;
            GameObject instanceGO = GameObject.Instantiate<GameObject>(fbxObject as GameObject);
            instanceGO.name = fbxObject.name;

            SkinnedMeshRenderer[] smrs = instanceGO.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach(var smr in smrs)
            {
                string mrd = string.Format("{0}/{1}_renderer.asset", targetAssetDir,/* instanceGO.name,*/ smr.name);
                AvatarRendererPart part = AssetDatabase.LoadAssetAtPath<AvatarRendererPart>(mrd);
                if(part == null)
                {
                    part = ScriptableObject.CreateInstance<AvatarRendererPart>();
                    AssetDatabase.CreateAsset(part, mrd);

                    AssetDatabase.ImportAsset(mrd);
                }
                part.rootBoneName = smr.rootBone.name;
                part.mesh = smr.sharedMesh;
                part.materials = smr.sharedMaterials;
                part.rendererNodeName = smr.name;
                part.boneNames = new string[smr.bones.Length];
                for(int i =0;i<smr.bones.Length;i++)
                {
                    part.boneNames[i] = smr.bones[i].name;
                }

                Mesh cMesh = UnityObject.Instantiate<Mesh>(part.mesh);
                List<Color> colors = null;
                cMesh.SetColors(colors);
                if (meshCompression != ModelImporterMeshCompression.Off)
                {
                    MeshUtility.SetMeshCompression(cMesh, meshCompression);
                }
                MeshUtility.Optimize(cMesh);

                string meshAssetPath = string.Format("{0}/{1}_mesh.asset", targetAssetDir, /*instanceGO.name,*/ smr.name);

                AssetDatabase.DeleteAsset(meshAssetPath);
                AssetDatabase.CreateAsset(cMesh, meshAssetPath);

                part.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshAssetPath);
            }
            AssetDatabase.SaveAssets();

            GameObject.DestroyImmediate(instanceGO);
        }

        public static void CreatePart(string targetAssetDir,AvatarRendererPart[] rendererParts = null)
        {
            string name = "avatar_part";
            if(rendererParts!=null && rendererParts.Length>0)
            {
                name = rendererParts[0].name.Replace("renderer", "part");
            }
            string assetPath = string.Format("{0}/{1}.asset", targetAssetDir, name);
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AvatarPart aPart = ScriptableObject.CreateInstance<AvatarPart>();
            AssetDatabase.CreateAsset(aPart, assetPath);

            aPart.rendererParts = rendererParts;

            AssetDatabase.SaveAssets();

            Selection.activeObject = aPart;
        }
    }
}
