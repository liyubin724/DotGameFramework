using Entitas;

public abstract class ABaseView
{
    protected Contexts contexts;
    protected Services services;
    protected IEntity entity;
    
    public virtual void InitializeView(Contexts contexts, Services services, IEntity entity)
    {
        this.contexts = contexts;
        this.services = services;
        this.entity = entity;
        AddListeners();
    }

    public virtual void DestroyView()
    {
        RemoveListeners();
        contexts = null;
        services = null;
        entity = null;
    }

    public abstract bool Active { get; set; }
    public abstract void AddListeners();
    public abstract void RemoveListeners();
}