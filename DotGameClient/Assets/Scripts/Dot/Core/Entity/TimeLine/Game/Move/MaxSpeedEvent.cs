using Dot.Core.Entity.Data;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Motion", "Max Speed", TimeLineExportPlatform.ALL)]
    public class MaxSpeedEvent : AEventItem
    {
        public float MaxSpeed { get; set; }

        private float cachedMaxSpeed = 0.0f;
        public override void DoRevert()
        {
            if (CanRevert&&entity.EntityData != null)
            {
                EntityMoveData moveData = entity.EntityData.MoveData;
                if (moveData != null)
                {
                    moveData.SetTimeLineMaxSpeed(cachedMaxSpeed);
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
                    cachedMaxSpeed = moveData.GetTimeLineMaxSpeed();
                    moveData.SetTimeLineMaxSpeed(MaxSpeed);
                }
            }
        }
    }
}
