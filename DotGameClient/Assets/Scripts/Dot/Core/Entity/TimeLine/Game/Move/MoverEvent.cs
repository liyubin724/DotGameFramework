using Dot.Core.Entity.Data;

namespace Dot.Core.TimeLine.Game
{
    [TimeLineMark("Event/Motion", "Mover", TimeLineExportPlatform.ALL)]
    public class MoverEvent : AEventItem
    {
        public bool IsMover { get; set; } = false;

        private bool cachedIsMover = false;

        public override void DoRevert()
        {
            if (entity.EntityData != null && CanRevert)
            {
                EntityMoveData moveData = entity.EntityData.MoveData;
                if (moveData != null)
                {
                    moveData.SetIsMover(cachedIsMover);
                }
            }
        }

        public override void Trigger()
        {
            if(entity.EntityData !=null)
            {
                EntityMoveData moveData = entity.EntityData.MoveData;
                if(moveData!=null)
                {
                    cachedIsMover = moveData.GetIsMover();
                    moveData.SetIsMover(IsMover);
                }
            }
        }
    }
}
