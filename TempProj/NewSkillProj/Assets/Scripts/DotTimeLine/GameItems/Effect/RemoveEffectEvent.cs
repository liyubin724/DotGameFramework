using DotTimeLine.Base.Items;
using DotTimeLine.Items;

namespace Game.TimeLine
{
    [TimeLineItem("Effect", "Remove Effect", TimeLineItemPlatform.Client)]
    public class RemoveEffectEvent : ATimeLineEventItem
    {
        [TimeLineDependOn]
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
