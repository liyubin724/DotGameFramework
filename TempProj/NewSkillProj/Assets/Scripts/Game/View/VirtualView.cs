using Entitas;

public abstract class VirtualView : IVirtualView
{
    private Contexts contexts;
    private Services services;
    private IEntity entity;
    
    public virtual void DestroyView()
    {
        RemoveListeners();
        contexts = null;
        services = null;
        entity = null;
    }

    public IEntity GetEntity()
    {
        return entity;
    }

    public Contexts GetContexts()
    {
        return contexts;
    }

    public Services GetServices()
    {
        return services;
    }

    public virtual void InitializeView(Contexts contexts, Services services, IEntity entity)
    {
        this.contexts = contexts;
        this.services = services;
        this.entity = entity;
        AddListeners();
    }

    public abstract bool Active { get; set; }
    public abstract void AddListeners();
    public abstract void RemoveListeners();
}