using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Motion", "Set Motion Curve", TimeLineExportPlatform.ALL)]
    public class SetMotionCurveTypeEvent : AEventItem
    {
        public MotionCurveType Motion { get; set; }
        public override void Trigger()
        {
            GetGameEntity().ReplaceMotionCurveType(Motion);
        }
    }
}
