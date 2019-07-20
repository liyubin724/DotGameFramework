using Entitas;
using System;
using System.Collections.Generic;

namespace Game.TimeLine.Systems
{
    public class TimeLinePlaySystem : AGameEntityReactiveSystem
    {
        public TimeLinePlaySystem(Contexts contexts, Services services) : base(contexts, services)
        {
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach(var e in entities)
            {
                e.timeLine.data.Play(e.timeLinePlay.groupName);
            }
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasTimeLine && entity.hasTimeLinePlay;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.TimeLine,GameMatcher.TimeLinePlay));
        }
    }
}
