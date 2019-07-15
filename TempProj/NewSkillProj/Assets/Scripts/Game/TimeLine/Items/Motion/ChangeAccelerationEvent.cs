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
            GetGameEntity().ReplaceAcceleration(Acceleration);
#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"ChangeAccelerationEvent::Trigger->Changed Acc.value = {Acceleration}");
#endif
        }
    }
}
