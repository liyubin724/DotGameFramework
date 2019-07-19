using Entitas;
using System.Collections.Generic;
using UnityEngine;


public class SkeletonSystem : AGameEntityReactiveSystem
{
    public SkeletonSystem(Contexts contexts, Services services) : base(contexts, services)
    {
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context) 
	{
        return context.CreateCollector(GameMatcher.Skeleton.AddedOrRemoved());
	}
		
	protected override bool Filter(GameEntity entity) 
	{
        return entity.hasView;
	}

	protected override void Execute(List<GameEntity> entities) 
	{
		foreach (var e in entities) 
		{
            if (e.view.view is ISkeletonView skeletonView)
            {
                if (e.hasSkeleton)
                {
                    GameObject skeletonPrefab = Resources.Load<GameObject>(e.skeleton.assetPath);
                    skeletonView.AddSkeleton(Object.Instantiate(skeletonPrefab));
                }
                else
                {
                    skeletonView.RemoveSkeleton();
                }
            }
        }
	}
}
