using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Mover", TimeLineExportPlatform.ALL)]
    public class MoverEvent : AEventItem
    {
        public bool IsMover { get; set; } = false;

        public override void Trigger()
        {
            (entity as GameEntity).isMover = IsMover;
#if DTL_DEBUG
            services.logService.Log(DebugLogType.Info, "DTLAddMoverEvent::DoEnter->Set Mover");
#endif
        }
    }
}
