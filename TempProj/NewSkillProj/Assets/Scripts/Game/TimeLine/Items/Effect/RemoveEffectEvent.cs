using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Effect", "Remove Effect", TimeLineExportPlatform.Client)]
    public class RemoveEffectEvent : AEventItem
    {
        [ItemDependOnAttribute(typeof(AddEffectEvent))]
        public int EffectIndex { get; set; }

        public override void Trigger()
        {
            GameEntity gameEntity = entity as GameEntity;
            var entities = contexts.game.GetEntitiesWithChildOf(gameEntity.uniqueID.value);
            foreach (var e in entities)
            {
                if (e.isEffect && e.hasTimeLineID && e.timeLineID.value == EffectIndex)
                {
                    e.isMarkDestroy = true;
#if DTL_DEBUG
                services.logService.Log(DebugLogType.Info, "DTLRemoveEffectEvent::DoEnter->Remove Effect");
#endif
                    break;
                }
            }
        }
    }
}
