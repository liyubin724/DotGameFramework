﻿using Dot.Core.TimeLine;
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
//        foreach (var e in gameEntityGroup.GetEntities())
//        {
//            if (e.timeLineController.data == null)
//                continue;
//            TimeLineData controller = e.timeLineController.data;
//            if(controller.State == TimeLineState.Invalid)
//            {
//                controller.Play();
//#if TIMELINE_DEBUG
//                services.logService.Log(DebugLogType.Info, "Start Play TimelineController");
//#endif
//            }else if(controller.State == TimeLineState.Finished)
//            {
//                e.isMarkDestroy = true;
//#if TIMELINE_DEBUG
//                services.logService.Log(DebugLogType.Info, "Finished TimelineController");
//#endif
//                continue;
//            }
//            controller.DoUpdate(services.timeService.DeltaTime());
//        }
    }
}
