using Entitas;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Physics
{
    public class ColliderSystem : AGameEntityReactiveSystem
    {
        public ColliderSystem(Contexts contexts, Services services) : base(contexts, services)
        {
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach(var entity in entities)
            {
                VirtualView view = entity.view.view as VirtualView;
                if(view !=null)
                {
                    Collider collider = view.GetOrCreateCollider(entity.collider.colliderType);
                    if(collider!=null)
                    {
                        if(entity.collider.colliderType == ColliderType.Capsule)
                        {
                            CapsuleCollider cc = collider as CapsuleCollider;
                            cc.center = entity.capsuleCollider.center;
                            cc.radius = entity.capsuleCollider.radius;
                            cc.height = entity.capsuleCollider.height;
                            cc.direction = entity.capsuleCollider.direction;
                            cc.isTrigger = entity.capsuleCollider.isTrigger;
                        }
                    }
                }
            }
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView && entity.hasCollider && (entity.hasCapsuleCollider);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Collider, GameMatcher.AnyOf(GameMatcher.CapsuleCollider)));
        }
    }
}
