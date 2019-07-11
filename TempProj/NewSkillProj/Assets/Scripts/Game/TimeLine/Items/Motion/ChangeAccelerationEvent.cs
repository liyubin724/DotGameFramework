using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Change Acceleration", TimeLineExportPlatform.ALL)]
    public class ChangeAccelerationEvent : AEventItem
    {
        public float Acceleration { get; set; }
        public override void Trigger()
        {
            (entity as GameEntity).ReplaceAcceleration(Acceleration);
#if DTL_DEBUG
            services.logService.Log(DebugLogType.Info, "DTLChangeAccelerationEvent::Trigger->Changed Acc");
#endif
        }
    }
}
