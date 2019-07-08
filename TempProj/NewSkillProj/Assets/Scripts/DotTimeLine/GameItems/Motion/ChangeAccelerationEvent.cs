using DotTimeLine.Base.Items;

namespace Game.TimeLine
{
    [TimeLineItem("Motion", "Change Acceleration", TimeLineItemPlatform.ALL)]
    public class ChangeAccelerationEvent : ATimeLineEventItem
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
