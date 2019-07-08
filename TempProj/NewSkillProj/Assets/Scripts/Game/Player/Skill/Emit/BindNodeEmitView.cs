using System;
using UnityEngine;

public class BindNodeEmitView : VirtualView,IBindNodeToView,IMarkDestroyListener
{
    public Transform BindNodeTransform { get; set; }
    public Vector3 Position => BindNodeTransform.position;
    public override bool Active { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void AddListeners()
    {
        ViewEntity.AddMarkDestroyListener(this);
    }

    public void OnMarkDestroy(GameEntity entity)
    {
        BindNodeTransform = null;
    }

    public override void RemoveListeners()
    {
        ViewEntity.RemoveMarkDestroyListener(this);
    }

    public void BindNode(GameObject node)
    {
        if(node!=null)
        {
            node.transform.SetParent(BindNodeTransform, false);
        }
    }

    private GameEntity ViewEntity
    {
        get
        {
            return (GameEntity)GetEntity();
        }
    }
}
