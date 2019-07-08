using Entitas;
using System.Collections.Generic;


public class MarkDestroySystem : ReactiveSystem<GameEntity> 
{
	private readonly Contexts contexts;
	private readonly Services services;

	public MarkDestroySystem (Contexts contexts,Services services) : base(contexts.game) 
	{
		this.contexts = contexts;
		this.services = services;
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
		foreach (var e in entities) 
		{
            e.Destroy();
		}
	}
}
