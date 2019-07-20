using Entitas;

namespace Game.TimeLine.Systems
{
    public class TimeLineUpdateSystem : AGameEntitySystem, IExecuteSystem,IInitializeSystem
    {
        IGroup<GameEntity> entityGroup = null;
        public TimeLineUpdateSystem(Contexts contexts, Services services) : base(contexts, services)
        {

        }

        public void Execute()
        {
            foreach(var e in entityGroup.GetEntities())
            {
                if(e.timeLine.data!=null)
                {
                    e.timeLine.data.DoUpdate(services.timeService.DeltaTime());
                }
            }
        }

        public void Initialize()
        {
            entityGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.TimeLine,GameMatcher.TimeLinePlay));
        }
    }
}
