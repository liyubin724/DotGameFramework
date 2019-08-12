using Entitas;
using UnityEngine;

public class VirtualView : ABaseView, IPositionListener, IDirectionListener, IMarkDestroyListener
{
    public GameObject RootGameObject
    {
        get; private set;
    }

    public Transform RootTransform
    {
        get; private set;
    }

    private Collider collider = null;
    public Collider GetOrCreateCollider(ColliderType colliderType)
    {
        if(collider == null)
        {
            collider = RootGameObject.GetComponent<Collider>();
        }
        if (colliderType == ColliderType.Capsule)
        {
            if(collider !=null)
            {
                if(collider.GetType() != typeof(CapsuleCollider))
                {
                    Debug.LogError("Collider not Same");
                    return null;
                }
            }else
            {
                collider = RootGameObject.AddComponent<CapsuleCollider>();
            }
        }

        return collider;
    }
    
    private Rigidbody rigidbody = null;
    public Rigidbody GetOrCreateRigidbody()
    {
        if(rigidbody == null)
        {
            rigidbody = RootGameObject.GetComponent<Rigidbody>();
        }
        if(rigidbody == null)
        {
            rigidbody = RootGameObject.AddComponent<Rigidbody>();
        }
        return rigidbody;
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
        EntityBehaviour idBehaviour = RootGameObject.AddComponent<EntityBehaviour>();
        idBehaviour.entity = entity;
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

    public void SetLayer(int layerMask)
    {
        RootGameObject.layer = layerMask;
    }

    protected GameEntity ViewEntity => (GameEntity)entity;
}
