using DotTimeLine.Base.Items;

namespace Game.TimeLine
{
    [TimeLineItem("Motion", "Change Speed", TimeLineItemPlatform.ALL)]
    public class ChangeSpeedEvent : ATimeLineEventItem
    {
        public float Speed { get; set; }
        public override void Trigger()
        {
            (entity as GameEntity).ReplaceSpeed(Speed);
        }
    }
}
