using Entitas;
using System.Collections.Generic;


public class MarkDestroySystem : AGameEntityReactiveSystem
{
    public MarkDestroySystem(Contexts contexts, Services services) : base(contexts, services)
    {
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) 
	{
		return context.CreateCollector(GameMatcher.MarkDestroy);
	}
		
	protected override bool Filter(GameEntity entity) 
	{
		return entity.isMarkDestroy;
	}

	protected override void Execute(List<GameEntity> entities) 
	{
        entities.ForEach((e) => e.Destroy());
	}
}
