using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Set Constant Speed", TimeLineExportPlatform.ALL)]
    public class SetConstantSpeedEvent : AEventItem
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
