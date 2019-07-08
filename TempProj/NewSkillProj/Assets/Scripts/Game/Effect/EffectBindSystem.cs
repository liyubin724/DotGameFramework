using Entitas;
using System.Collections.Generic;


public class EffectBindSystem : ReactiveSystem<GameEntity> 
{
	private readonly Contexts contexts;
	private readonly Services services;

	public EffectBindSystem (Contexts contexts,Services services) : base(contexts.game) 
	{
		this.contexts = contexts;
		this.services = services;
	}
	
	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) 
	{
		return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Effect,GameMatcher.EffectBind,GameMatcher.ChildOf));
	}
		
	protected override bool Filter(GameEntity entity) 
	{
		return entity.isEffect&&entity.hasEffectBind;
	}

	protected override void Execute(List<GameEntity> entities) 
	{
		foreach (var e in entities) 
		{
            GameEntity bindEntity = contexts.game.GetEntityWithUniqueID(e.childOf.entityID);
            INodeBehaviourView bindView = bindEntity.virtualView.value as INodeBehaviourView;
            //bindView.AddNodeBind((e.virtualView.value as EffectView).EffectGameObject, e.effectBind.nodeType, e.effectBind.bindIndex);
		}
	}
}
