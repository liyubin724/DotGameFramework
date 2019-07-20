using Entitas;
using System.Collections.Generic;

namespace Game.TimeLine.Systems
{
    public class TimeLineDataSystem : AGameEntityReactiveSystem
    {
        public TimeLineDataSystem(Contexts contexts, Services services) : base(contexts, services)
        {
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach(var e in entities)
            {
                if(e.hasTimeLine)
                {
                    e.timeLine.data.Initialize(contexts, services, e);
                    e.timeLine.data.groupFinishCallback = OnGroupFinish;
                }
            }
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasTimeLine;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.TimeLine);
        }

        private void OnGroupFinish(IEntity entity,string groupName)
        {
            GameEntity gameEntity = (GameEntity)entity;
            gameEntity.RemoveTimeLinePlay();
            gameEntity.AddTimeLinePlayFinish(groupName);
        }

        private void OnGroupStart(IEntity entity,string groupName)
        {

        }

        private void OnGroupChanged(IEntity entity,string preGroupName,string curGroupName)
        {

        }
    }
}
