using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Dot.Core.Entity
{
    [Serializable]
    public class EntityBindNodeData
    {
        public string atlasName = "";
        public Transform transform = null;
        public Vector3 postionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero;
    }

    [Serializable]
    public class EntityMeshRendererNodeData
    {
        public string name = "";
        public SkinnedMeshRenderer renderer = null;
    }

    [Serializable]
    public class EntityBoneNodeData
    {
        public string name = null;
        public Transform transform = null;
    }

    public class EntityNodeBehaviour : MonoBehaviour
    {
        [ReadOnly]
        public EntityBoneNodeData[] boneNodes = new EntityBoneNodeData[0];
        [ReadOnly]
        public EntityMeshRendererNodeData[] rendererNodes = new EntityMeshRendererNodeData[0];
        public EntityBindNodeData[] bindNodes = new EntityBindNodeData[0];

        private Dictionary<string, EntityBoneNodeData> boneNodeDic = null;
        public EntityBoneNodeData GetBoneNode(string name)
        {
            if(boneNodeDic == null)
            {
                boneNodeDic = new Dictionary<string, EntityBoneNodeData>();
                foreach(var node in boneNodes)
                {
                    boneNodeDic.Add(node.name, node);
                }
            }

            if(boneNodeDic.TryGetValue(name,out EntityBoneNodeData bNode))
            {
                return bNode;
            }
            return null;
        }

        private Dictionary<string, EntityMeshRendererNodeData> rendererNodeDic = null;
        public EntityMeshRendererNodeData GetRendererNode(string name)
        {
            if(rendererNodeDic == null)
            {
                rendererNodeDic = new Dictionary<string, EntityMeshRendererNodeData>();
                foreach(var n in rendererNodes)
                {
                    rendererNodeDic.Add(n.name, n);
                }
            }
            if(rendererNodeDic.TryGetValue(name,out EntityMeshRendererNodeData node))
            {
                return node;
            }
            return null;
        }

        private Dictionary<string, EntityBindNodeData> bindNodeDic = null;
        public EntityBindNodeData GetBindNode(string name)
        {
            if (bindNodeDic == null)
            {
                bindNodeDic = new Dictionary<string, EntityBindNodeData>();
                foreach (var n in bindNodes)
                {
                    bindNodeDic.Add(n.atlasName, n);
                }
            }
            if (bindNodeDic.TryGetValue(name, out EntityBindNodeData node))
            {
                return node;
            }
            return null;
        }

        public Transform[] GetTransformByNames(string[] names)
        {
            if (names == null || names.Length == 0)
                return new Transform[0];

            Transform[] transforms = new Transform[names.Length];
            for(int i =0;i<names.Length;i++)
            {
                transforms[i] = GetBoneNode(names[i])?.transform;
            }
            return transforms;
        }
    }
}
