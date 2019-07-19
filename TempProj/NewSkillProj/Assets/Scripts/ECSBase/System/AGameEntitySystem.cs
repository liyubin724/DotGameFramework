using Entitas;

public abstract class AGameEntitySystem
{
    protected Contexts contexts;
    protected Services services;

    public AGameEntitySystem(Contexts contexts,Services services)
    {
        this.contexts = contexts;
        this.services = services;
    }

    public IGroup<GameEntity> GetGroup(IMatcher<GameEntity> matcher)=> contexts.game.GetGroup(matcher);
}