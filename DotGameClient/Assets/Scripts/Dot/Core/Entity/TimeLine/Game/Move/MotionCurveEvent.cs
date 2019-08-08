using Dot.Core.Entity.Data;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Motion", "Curve", TimeLineExportPlatform.ALL)]
    public class MotionCurveEvent : AEventItem
    {
        public MotionCurveType Motion { get; set; }

        private MotionCurveType cachedMotion = MotionCurveType.None;
        public override void DoRevert()
        {
            if (CanRevert&&entity.EntityData != null)
            {
                EntityMoveData moveData = entity.EntityData.MoveData;
                if (moveData != null)
                {
                    moveData.SetMotionType(cachedMotion);
                }
            }
        }

        public override void Trigger()
        {
            if (entity.EntityData != null)
            {
                EntityMoveData moveData = entity.EntityData.MoveData;
                if (moveData != null)
                {
                    cachedMotion = moveData.GetMotionType();
                    moveData.SetMotionType(cachedMotion);
                }
            }
        }
    }
}
