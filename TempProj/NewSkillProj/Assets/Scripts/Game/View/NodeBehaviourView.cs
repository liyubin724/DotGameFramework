using UnityEngine;

public class NodeBehaviourView : SkeletonView, INodeBehaviourView
{
    public NodeBehaviourView(string name) : base(name)
    {
    }

    public NodeBehaviourView(string name, Transform parent) : base(name, parent)
    {
    }

    public void AddNodeBind(GameObject bindGO, BindNodeType nodeType, int nodeIndex)
    {
        if(GetNodeBehaviour()!=null)
        {
            Transform bindNodeTransform = GetNodeBehaviour().GetBindNode(nodeType)[nodeIndex].nodeTransform;
            bindGO.transform.SetParent(bindNodeTransform, false);
        }
    }

    public int GetNodeBindCount(BindNodeType nodeType) => GetNodeBehaviour() == null ? 0 : GetNodeBehaviour().GetBindNodeCount(nodeType);

    public BindNodeData GetNodeBindData(BindNodeType nodeType, int nodeIndex) => GetNodeBehaviour()?.GetBindNode(nodeType)[nodeIndex];

    public void RemoveNodeBind(BindNodeType nodeType, int nodeIndex)
    {
        BindNodeData nodeData = GetNodeBindData(nodeType, nodeIndex);
        int count = nodeData.nodeTransform.childCount;
        for(int i =0;i<count;i++)
        {
            Transform cTran = nodeData.nodeTransform.GetChild(0);
            Object.Destroy(cTran.gameObject);
        }
    }

    private NodeBehaviour nodeBehaviour = null;
    private NodeBehaviour GetNodeBehaviour()
    {
        if (nodeBehaviour == null && skeletonGameObject != null)
        {
            nodeBehaviour = skeletonGameObject.GetComponent<NodeBehaviour>();
        }
        return nodeBehaviour;
    }
}