using Dot.Core.Entity.Data;
using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Motion", "Constant Speed", TimeLineExportPlatform.ALL)]
    public class ConstantSpeedEvent : AEventItem
    {
        public float Speed { get; set; }

        private float cachedSpeed = 0.0f;
        public override void DoRevert()
        {
            if (entity.EntityData != null)
            {
                EntityMoveData moveData = entity.EntityData.MoveData;
                if (moveData != null)
                {
                    moveData.SetTimeLineConstantSpeed(cachedSpeed);
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
                    cachedSpeed = moveData.GetTimeLineConstantSpeed();
                    moveData.SetTimeLineConstantSpeed(Speed);
                }
            }
        }
    }
}
