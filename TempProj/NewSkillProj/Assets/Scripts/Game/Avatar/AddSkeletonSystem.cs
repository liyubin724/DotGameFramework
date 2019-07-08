using Entitas;
using System.Collections.Generic;
using UnityEngine;


public class AddSkeletonSystem : ReactiveSystem<GameEntity> 
{
	private readonly Contexts contexts;
	private readonly Services services;

	public AddSkeletonSystem (Contexts contexts,Services services) : base(contexts.game) 
	{
		this.contexts = contexts;
		this.services = services;
	}
		
	protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) 
	{
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.AddSkeleton,GameMatcher.VirtualView));
	}
		
	protected override bool Filter(GameEntity entity) 
	{
		return entity.hasAddSkeleton && entity.hasVirtualView;
	}

	protected override void Execute(List<GameEntity> entities) 
	{
		foreach (var e in entities) 
		{
            ISkeletonView skeletonView = e.virtualView.value as ISkeletonView;
            if(skeletonView!=null)
            {
                GameObject skeletonPrefab = Resources.Load<GameObject>(e.addSkeleton.skeletonPath);
                skeletonView.AddSkeleton(Object.Instantiate(skeletonPrefab));
            }
            e.RemoveAddSkeleton();
		}
	}
}
