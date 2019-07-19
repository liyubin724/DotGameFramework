using Entitas;

public abstract class GameEntityReactiveSystem : ReactiveSystem<GameEntity>
{
    protected readonly Contexts contexts;
    protected readonly Services services;

    public GameEntityReactiveSystem(Contexts contexts, Services services) : base(contexts.game)
    {
        this.contexts = contexts;
        this.services = services;
    }
}