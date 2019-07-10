using DotTimeLine.Base.Items;

namespace Game.TimeLine
{
    [TimeLineItem("Motion", "Mover", TimeLineItemPlatform.ALL)]
    public class MoverEvent : ATimeLineEventItem
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
