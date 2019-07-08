using UnityEngine;

public class PackRootView : VirtualView,IMarkDestroyListener,IPositionListener,ISkeletonView,INodeBehaviourView
{
    private GameObject skeletonGameObject = null;
    private NodeBehaviour nodeBehaviour = null;

    public GameObject RootGameObject
    {
        get; private set;
    }

    public Transform RootTransform
    {
        get; private set;
    }

    public override bool Active
    {
        get
        {
            return RootGameObject.activeSelf;
        }
        set
        {
            RootGameObject.SetActive(value);
        }
    }

    public PackRootView(string name):this(name,null)
    {
    }

    public PackRootView(string name,Transform parent)
    {
        RootGameObject = new GameObject(name);
        RootTransform = RootGameObject.transform;

        if(parent!=null)
        {
            RootTransform.SetParent(parent, false);
        }
    }

    public override void AddListeners()
    {
        ViewEntity.AddMarkDestroyListener(this);
        ViewEntity.AddPositionListener(this);
    }

    public override void RemoveListeners()
    {
        ViewEntity.RemoveMarkDestroyListener(this);
        ViewEntity.RemovePositionListener(this);
    }

    public void OnMarkDestroy(GameEntity entity)
    {
        skeletonGameObject = null;
        if(RootGameObject!=null)
            GameObject.Destroy(RootGameObject);
        RootGameObject = null;
        RootTransform = null;

        DestroyView();
    }

    public void OnPosition(GameEntity entity, Vector3 value)
    {
        RootTransform.position = value;
    }

    public void AddSkeleton(GameObject go)
    {
        skeletonGameObject = go;
        skeletonGameObject.transform.SetParent(RootTransform, false);
    }

    public void RemoveSkeleton()
    {
        if(skeletonGameObject!=null)
        {
            GameObject.Destroy(skeletonGameObject);
        }
        skeletonGameObject = null;
        nodeBehaviour = null;
    }

    public NodeBehaviour GetNodeBehaviour()
    {
        if(nodeBehaviour == null && skeletonGameObject!=null)
        {
            nodeBehaviour = skeletonGameObject.GetComponent<NodeBehaviour>();
        }
        return nodeBehaviour;
    }

    public void AddNodeBind(GameObject bindGO, BindNodeType nodeType, int nodeIndex)
    {
        Transform bindNodeTransform = GetNodeBehaviour().GetBindNode(nodeType)[nodeIndex].nodeTransform;
        bindGO.transform.SetParent(bindNodeTransform, false);
    }

    public void RemoveNodeBind(BindNodeType nodeType, int nodeIndex)
    {
        
    }

    public BindNodeData GetNodeBindData(BindNodeType nodeType, int nodeIndex)
    {
        return GetNodeBehaviour().GetBindNode(nodeType)[nodeIndex];
    }

    protected GameEntity ViewEntity
    {
        get
        {
            return (GameEntity)GetEntity();
        }
    }
}