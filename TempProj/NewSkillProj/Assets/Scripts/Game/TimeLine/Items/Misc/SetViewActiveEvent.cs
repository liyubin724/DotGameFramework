using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Misc", "Set View Active", TimeLineExportPlatform.Client)]
    public class SetViewActiveEvent : AEventItem
    {
        public bool IsActive { get; set; } = true;

        public override void Trigger()
        {
            GameEntity entity = GetGameEntity();
            if(entity.hasVirtualView)
            {
                entity.virtualView.value.Active = IsActive;
            }
        }
    }
}
