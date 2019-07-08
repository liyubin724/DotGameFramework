using System.Collections.Generic;
using UnityEngine;

public class NodeBehaviour : MonoBehaviour
{
    public List<BindNodeData> bindNodeList = new List<BindNodeData>();

    private Dictionary<BindNodeType, List<BindNodeData>> bindNodeDic = new Dictionary<BindNodeType, List<BindNodeData>>();
    private void Awake()
    {
        foreach(var n in bindNodeList)
        {
            if(!bindNodeDic.TryGetValue(n.nodeType,out List<BindNodeData> nodeList))
            {
                nodeList = new List<BindNodeData>();
                bindNodeDic.Add(n.nodeType, nodeList);
            }

            nodeList.Add(n);
        }
    }

    public int GetBindNodeCount(BindNodeType nodeType)
    {
        if(bindNodeDic.TryGetValue(nodeType,out List<BindNodeData> nodeList))
        {
            return nodeList.Count;
        }
        return 0;
    }

    public BindNodeData[] GetBindNode(BindNodeType nodeType)
    {
        if (bindNodeDic.TryGetValue(nodeType, out List<BindNodeData> nodeList))
        {
            return nodeList.ToArray();
        }
        return null;
    }

}