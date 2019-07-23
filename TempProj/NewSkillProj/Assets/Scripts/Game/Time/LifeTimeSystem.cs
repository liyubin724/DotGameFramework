using Entitas;

//namespace Game.Time

public class LifeTimeSystem : AGameEntitySystem, IInitializeSystem, IExecuteSystem
{
    private IGroup<GameEntity> entityGroup = null;
    public LifeTimeSystem(Contexts contexts, Services services) : base(contexts, services)
    {
    }

    public void Execute()
    {
        foreach (var entity in entityGroup.GetEntities())
        {
            entity.ReplaceLifeTime(entity.lifeTime.time - services.timeService.DeltaTime());
            if (entity.lifeTime.time <= 0)
            {
                entity.isMarkDestroy = true;
            }
        }
    }

    public void Initialize()
    {
        entityGroup = GetGroup(GameMatcher.LifeTime);
    }
}

