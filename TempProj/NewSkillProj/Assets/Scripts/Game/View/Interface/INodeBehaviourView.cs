using UnityEngine;

public interface INodeBehaviourView
{
    int GetNodeBindCount(BindNodeType nodeType);
    BindNodeData GetNodeBindData(BindNodeType nodeType, int nodeIndex);
    void AddNodeBind(GameObject bindGO, BindNodeType nodeType, int nodeIndex);
    void RemoveNodeBind(BindNodeType nodeType, int nodeIndex);
}