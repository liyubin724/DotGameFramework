using Entitas;
using Game.TimeLine;
using System.Collections.Generic;

namespace Game.Physics
{
    public class TriggerEnterSystem : AGameEntityReactiveSystem
    {
        public TriggerEnterSystem(Contexts contexts, Services services) : base(contexts, services)
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
                    }
                }

                entity.RemoveTriggerEnter();
            }
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasTriggerEnter;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.TriggerEnter);
        }
    }
}
