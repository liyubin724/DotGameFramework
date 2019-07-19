using Entitas;
using UnityEngine;

public class VirtualView : ABaseView,IPositionListener,IDirectionListener,IMarkDestroyListener
{
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

    public VirtualView(string name, Transform parent)
    {
        RootGameObject = new GameObject(name);
        RootTransform = RootGameObject.transform;

        if (parent != null)
        {
            RootTransform.SetParent(parent, false);
        }
    }

    public VirtualView(string name) : this(name, null)
    {
    }

    public override void InitializeView(Contexts contexts, Services services, IEntity entity)
    {
        base.InitializeView(contexts, services, entity);
        EntityUniqueIDBehaviour idBehaviour = RootGameObject.AddComponent<EntityUniqueIDBehaviour>();

        idBehaviour.enityID = ViewEntity.uniqueID.value;
    }

    public override void AddListeners()
    {
        ViewEntity.AddMarkDestroyListener(this);
        ViewEntity.AddPositionListener(this);
        ViewEntity.AddDirectionListener(this);
    }

    public override void RemoveListeners()
    {
        ViewEntity.RemoveMarkDestroyListener(this);
        ViewEntity.RemovePositionListener(this);
        ViewEntity.RemoveDirectionListener(this);
    }

    public void OnMarkDestroy(GameEntity entity)
    {
        if (RootGameObject != null)
            Object.Destroy(RootGameObject);

        RootGameObject = null;
        RootTransform = null;

        DestroyView();
    }

    public void OnPosition(GameEntity entity, Vector3 value)
    {
        RootTransform.position = value;
    }

    public void OnDirection(GameEntity entity, Vector3 value)
    {
        RootTransform.forward = value;
    }

    protected GameEntity ViewEntity => (GameEntity)entity;
}
