using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineItem("Motion", "Change Motion", TimeLineItemPlatform.ALL)]
    public class ChangeMotionTypeEvent : ATimeLineEventItem
    {
        public MotionType Motion { get; set; }
        public override void Trigger()
        {
            (entity as GameEntity).ReplaceMotionType(Motion);
        }
    }
}
