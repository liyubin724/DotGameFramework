using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;


public class EmitSkillSystem : ReactiveSystem<GameEntity> 
{
	private readonly Contexts contexts;
	private readonly Services services;

	public EmitSkillSystem (Contexts contexts,Services services) : base(contexts.game) 
	{
		this.contexts = contexts;
		this.services = services;
	}
		
	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) 
	{
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Player, GameMatcher.EmitSkill));
	}
		
	protected override bool Filter(GameEntity entity) 
	{
		return entity.isPlayer && entity.hasEmitSkill;
	}

	protected override void Execute(List<GameEntity> entities) 
	{
		foreach (var e in entities) 
		{
            services.entityFactroy.CreateSkillEntity(e, e.emitSkill.skillID);
            e.RemoveEmitSkill();
		}
	}
}
