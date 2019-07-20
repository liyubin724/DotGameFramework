using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Motion", "Flow Owner Speed", TimeLineExportPlatform.ALL)]
    public class SetFlowOwnerSpeedEvent : AEventItem
    {
        public override void Trigger()
        {
            GameEntity entity = GetGameEntity();
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(entity.ownerID.value);
            if(ownerEntity.hasSpeed)
            {
                entity.ReplaceSpeed(ownerEntity.speed.value);
#if TIMELINE_DEBUG
                services.logService.Log(DebugLogType.Info, $"SetFlowOwnerSpeedEvent::Trigger->Flow Owner Speed. speed = {ownerEntity.speed.value}");
#endif
            }
            else
            {
                entity.ReplaceSpeed(0);

#if TIMELINE_DEBUG
                services.logService.Log(DebugLogType.Warning, $"SetFlowOwnerSpeedEvent::Trigger->No Speed. ");
#endif
            }
        }
    }
}
