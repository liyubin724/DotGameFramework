using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Change Speed", TimeLineExportPlatform.ALL)]
    public class ChangeSpeedEvent : AEventItem
    {
        public float Speed { get; set; }
        public override void Trigger()
        {
            GetGameEntity().ReplaceSpeed(Speed);

#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"ChangeSpeedEvent::Trigger->Change speed.value = {Speed}");
#endif
        }
    }
}
