using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Motion", "Set Mover", TimeLineExportPlatform.ALL)]
    public class SetMoverEvent : AEventItem
    {
        public bool IsMover { get; set; } = false;

        private bool cachedIsMover = false;
        public override void Trigger()
        {
            cachedIsMover = GetGameEntity().isMover;
            GetGameEntity().isMover = IsMover;
#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"ChangeMoverEvent::Trigger->Set Mover.IsMover = {IsMover}");
#endif
        }

        public override void DoRevert()
        {
            GetGameEntity().isMover = cachedIsMover;
        }

        public override void DoReset()
        {
            cachedIsMover = false;
            base.DoReset();
        }
    }
}
