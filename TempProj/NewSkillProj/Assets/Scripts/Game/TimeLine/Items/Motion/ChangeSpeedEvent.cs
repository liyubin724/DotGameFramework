using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Change Speed", TimeLineExportPlatform.ALL)]
    public class ChangeSpeedEvent : ATimeLineEventItem
    {
        public float Speed { get; set; }
        public override void Trigger()
        {
            (entity as GameEntity).ReplaceSpeed(Speed);
        }
    }
}
