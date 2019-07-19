using Entitas;

public interface IVirtualView
{
    IEntity GetEntity();
    bool Active { set; get; }
    void InitializeView(Contexts contexts, Services services, IEntity entity);
    void DestroyView();
    void AddListeners();
    void RemoveListeners();
}