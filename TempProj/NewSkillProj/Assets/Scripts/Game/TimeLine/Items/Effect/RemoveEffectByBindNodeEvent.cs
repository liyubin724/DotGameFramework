using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;
using Entitas;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Effect", "Remove Bind Node Effect", TimeLineExportPlatform.Client)]
    public class RemoveEffectByBindNodeEvent : AEventItem
    {
        public int NodeIndex { get; set; } = 0;
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int ConfigID { get; set; }

        public override void Trigger()
        {
            GameEntity skillEntity = GetGameEntity();
            IGroup<GameEntity> entityGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Effect, GameMatcher.BindNodeEffect));

#if TIMELINE_DEBUG
            bool isFound = false;
#endif
            foreach(var entity in entityGroup.GetEntities())
            {
                if(entity.configID.value == ConfigID && 
                    entity.bindNodeEffect.nodeIndex == NodeIndex && 
                    entity.bindNodeEffect.nodeType == NodeType)
                {
                    entity.isMarkDestroy = true;
#if TIMELINE_DEBUG
                    isFound = true;
#endif
                    break;
                }
            }

#if TIMELINE_DEBUG
            if(isFound)
            {
                services.logService.Log(DebugLogType.Info, $"RemoveBindNodeEffectEvent::Trigger->Remove Effect Success. configId = {ConfigID}, index = {NodeIndex},type = {NodeType}");
            }
            else
            {
                services.logService.Log(DebugLogType.Error, $"RemoveBindNodeEffectEvent::Trigger->The Effect not found. configId = {ConfigID}, index = {NodeIndex},type = {NodeType}");
            }
#endif

        }
    }
}
