using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Dot.Core.Entity
{
    [Serializable]
    public class BindNodeData
    {
        public string atlasName = "";
        public Transform transform = null;
        public Vector3 postionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero;
    }

    [Serializable]
    public class MeshRendererNodeData
    {
        public string name = "";
        public SkinnedMeshRenderer renderer = null;
    }

    [Serializable]
    public class BoneNodeData
    {
        public string name = null;
        public Transform transform = null;
    }

    public class NodeBehaviour : MonoBehaviour
    {
        [ReadOnly]
        public BoneNodeData[] boneNodes = new BoneNodeData[0];
        [ReadOnly]
        public MeshRendererNodeData[] rendererNodes = new MeshRendererNodeData[0];
        public BindNodeData[] bindNodes = new BindNodeData[0];

        private Dictionary<string, BoneNodeData> boneNodeDic = null;
        public BoneNodeData GetBoneNode(string name)
        {
            if(boneNodeDic == null)
            {
                boneNodeDic = new Dictionary<string, BoneNodeData>();
                foreach(var node in boneNodes)
                {
                    boneNodeDic.Add(node.name, node);
                }
            }

            if(boneNodeDic.TryGetValue(name,out BoneNodeData bNode))
            {
                return bNode;
            }
            return null;
        }

        private Dictionary<string, MeshRendererNodeData> rendererNodeDic = null;
        public MeshRendererNodeData GetRendererNode(string name)
        {
            if(rendererNodeDic == null)
            {
                rendererNodeDic = new Dictionary<string, MeshRendererNodeData>();
                foreach(var n in rendererNodes)
                {
                    rendererNodeDic.Add(n.name, n);
                }
            }
            if(rendererNodeDic.TryGetValue(name,out MeshRendererNodeData node))
            {
                return node;
            }
            return null;
        }

        private Dictionary<string, BindNodeData> bindNodeDic = null;
        public BindNodeData GetBindNode(string name)
        {
            if (bindNodeDic == null)
            {
                bindNodeDic = new Dictionary<string, BindNodeData>();
                foreach (var n in bindNodes)
                {
                    bindNodeDic.Add(n.atlasName, n);
                }
            }
            if (bindNodeDic.TryGetValue(name, out BindNodeData node))
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
