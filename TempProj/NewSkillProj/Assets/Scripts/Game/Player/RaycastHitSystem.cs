using Entitas;
using Game.TimeLine;
using System.Collections.Generic;

namespace Game.Player
{
    public class RaycastHitSystem : AGameEntityReactiveSystem
    {
        public RaycastHitSystem(Contexts contexts, Services services) : base(contexts, services)
        {
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach(var entity in entities)
            {
                if(entity.isBullet)
                {
                    if(entity.hasTimeLine)
                    {
                        entity.ReplaceTimeLinePlay(BulletTimeLineConst.TIMELINE_END);
                        entity.RemoveCollider();
                        entity.RemoveRigidbody();
                        if (entity.hasCapsuleCollider)
                            entity.RemoveCapsuleCollider();

                        services.logService.Log(DebugLogType.Error, $"name={(entity.view.view as VirtualView).RootGameObject.name},HitNmae = {entity.raycatHit.hit.collider.name}");
                    }
                }
            }
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasRaycatHit;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.RaycatHit);
        }
    }
}
