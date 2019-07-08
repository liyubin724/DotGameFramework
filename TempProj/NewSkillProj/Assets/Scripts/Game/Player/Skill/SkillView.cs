using System;
using UnityEngine;

public class SkillView : VirtualView, IMarkDestroyListener,INodeBehaviourView
{
    private PlayerView playerView = null;
    public override bool Active { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    private GameEntity ViewEntity
    {
        get
        {
            return (GameEntity)GetEntity();
        }
    }

    public void SetPlayerView(PlayerView pv)
    {
        playerView = pv;
    }

    public override void AddListeners()
    {
        ViewEntity.AddMarkDestroyListener(this);
    }

    public void AddNodeBind(GameObject bindGO, BindNodeType nodeType, int nodeIndex)
    {
        playerView.AddNodeBind(bindGO, nodeType, nodeIndex);
    }

    public void OnMarkDestroy(GameEntity entity)
    {
        DestroyView();
        playerView = null;
    }

    public override void RemoveListeners()
    {
        ViewEntity.RemoveMarkDestroyListener(this);
    }

    public void RemoveNodeBind(BindNodeType nodeType, int nodeIndex)
    {
    }

    public NodeBehaviour GetNodeBehaviour()
    {
        throw new NotImplementedException();
    }

    public BindNodeData GetNodeBindData(BindNodeType nodeType, int nodeIndex)
    {
        throw new NotImplementedException();
    }
}