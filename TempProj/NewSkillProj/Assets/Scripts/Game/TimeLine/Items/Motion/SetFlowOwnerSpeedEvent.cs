using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Motion", "Flow Owner Speed", TimeLineExportPlatform.ALL)]
    public class SetFlowOwnerSpeedEvent : AEventItem
    {
        private float cachedSpeed = 0.0f;
        private bool hasSpeed = false;
        public override void Trigger()
        {
            GameEntity entity = GetGameEntity();
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(entity.ownerID.value);

            hasSpeed = GetGameEntity().hasSpeed;
            if (hasSpeed)
            {
                cachedSpeed = GetGameEntity().speed.value;
            }

            if (ownerEntity.hasSpeed)
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

        public override void DoRevert()
        {
            if (!hasSpeed)
            {
                GetGameEntity().RemoveSpeed();
            }
            else
            {
                GetGameEntity().ReplaceSpeed(cachedSpeed);
            }
        }

        public override void DoReset()
        {
            cachedSpeed = 0.0f;
            hasSpeed = false;
            base.DoReset();
        }
    }
}
