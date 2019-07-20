using Entitas;
using System.Collections.Generic;

namespace Game.TimeLine.Systems
{
    public class TimeLineStopSystem : AGameEntityReactiveSystem
    {
        public TimeLineStopSystem(Contexts contexts, Services services) : base(contexts, services)
        {
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach(var e in entities)
            {
                if(e.hasTimeLinePlay)
                {
                    e.timeLine.data.Stop();
                    e.RemoveTimeLinePlay();
                }
                e.timeLine.data.DoReset();
                e.RemoveTimeLine();
                e.isTimeLineStop = false;
                e.isMarkDestroy = true;
            }
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasTimeLine && entity.isTimeLineStop;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.TimeLine, GameMatcher.TimeLineStop));
        }
    }
}
