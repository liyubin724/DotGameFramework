using System;
using UnityEngine;
using System.Collections.Generic;


namespace Dot.Core.Entity
{
    public enum BindNodeType
    {
        Main = 1,
        Sub,
        Super,
        Furnace,
    }


    [Serializable]
    public class BindNodeData
    {
        public BindNodeType nodeType = BindNodeType.Main;
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
        public BoneNodeData[] boneNodes = new BoneNodeData[0];
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

        private Dictionary<BindNodeType, List<BindNodeData>> bindNodeDic = null;

        public BindNodeData[] GetBindNodes(BindNodeType nodeType)
        {
            if (bindNodeDic == null)
            {
                bindNodeDic = new Dictionary<BindNodeType, List<BindNodeData>>();

                foreach (var n in bindNodes)
                {
                    if(!bindNodeDic.TryGetValue(n.nodeType,out List<BindNodeData> dataList))
                    {
                        dataList = new List<BindNodeData>();
                        bindNodeDic.Add(n.nodeType, dataList);
                    }
                    dataList.Add(n);
                }
            }
            if (bindNodeDic.TryGetValue(nodeType, out List<BindNodeData> nodeDataList))
            {
                return nodeDataList.ToArray();
            }
            return null;
        }

        public Transform[] GetBoneTransformByNames(string[] names)
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
