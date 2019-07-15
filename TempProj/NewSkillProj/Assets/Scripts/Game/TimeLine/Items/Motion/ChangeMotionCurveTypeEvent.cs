using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Motion", "Change Motion", TimeLineExportPlatform.ALL)]
    public class ChangeMotionCurveTypeEvent : AEventItem
    {
        public MotionCurveType Motion { get; set; }
        public override void Trigger()
        {
            GetGameEntity().ReplaceMotionCurveType(Motion);
        }
    }
}
