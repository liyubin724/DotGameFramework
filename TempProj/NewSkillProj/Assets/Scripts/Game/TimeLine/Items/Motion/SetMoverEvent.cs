using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Motion", "Set Mover", TimeLineExportPlatform.ALL)]
    public class SetMoverEvent : AEventItem
    {
        public bool IsMover { get; set; } = false;

        public override void Trigger()
        {
            GetGameEntity().isMover = IsMover;
#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"ChangeMoverEvent::Trigger->Set Mover.IsMover = {IsMover}");
#endif
        }
    }
}
