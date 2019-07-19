using Entitas;

public abstract class AGameEntityReactiveSystem : ReactiveSystem<GameEntity>
{
    protected readonly Contexts contexts;
    protected readonly Services services;

    public AGameEntityReactiveSystem(Contexts contexts, Services services) : base(contexts.game)
    {
        this.contexts = contexts;
        this.services = services;
    }
}