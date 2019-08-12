using Dot.Core.Entity.Data;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Motion", "Acceleration", TimeLineExportPlatform.ALL)]
    public class AccelerationEvent : AEventItem
    {
        public float Acceleration { get; set; }

        private float cachedAcc;
        public override void DoRevert()
        {
            if (CanRevert&&entity.EntityData != null)
            {
                EntityMoveData moveData = (entity.EntityData as IMoveData)?.GetMoveData();
                if (moveData != null)
                {
                    moveData.SetTimeLineAcceleration(cachedAcc);
                }
            }
        }

        public override void Trigger()
        {
            if (entity.EntityData != null)
            {
                EntityMoveData moveData = (entity.EntityData as IMoveData)?.GetMoveData();
                if (moveData != null)
                {
                    cachedAcc = moveData.GetTimeLineAcceleration();
                    moveData.SetTimeLineAcceleration(Acceleration);
                }
            }
        }
    }
}
