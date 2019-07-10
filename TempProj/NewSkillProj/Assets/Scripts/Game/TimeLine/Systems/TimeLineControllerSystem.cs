using Dot.Core.TimeLine.Base;
using Entitas;

public class TimeLineControllerSystem : IExecuteSystem
{
    private readonly Contexts contexts;
    private readonly Services services;
    private IGroup<GameEntity> gameEntityGroup = null;
    public TimeLineControllerSystem(Contexts contexts, Services services)
    {
        this.contexts = contexts;
        this.services = services;
        gameEntityGroup = contexts.game.GetGroup(GameMatcher.TimeLineController);

    }

    public void Execute()
    {
        foreach (var e in gameEntityGroup.GetEntities())
        {
            if (e.timeLineController.controller == null)
                continue;
            TimeLineController controller = e.timeLineController.controller;
            if(controller.State == TimeLineState.Invalid)
            {
                controller.Play();
                services.logService.Log(DebugLogType.Info, "Start Play TimelineController");
            }else if(controller.State == TimeLineState.Finished)
            {
                e.isMarkDestroy = true;
                services.logService.Log(DebugLogType.Info, "Finished TimelineController");
                continue;
            }
            controller.DoUpdate(services.timeService.DeltaTime());
        }
    }
}
