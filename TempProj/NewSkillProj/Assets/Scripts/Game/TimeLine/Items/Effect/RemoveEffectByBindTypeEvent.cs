using Dot.Core.TimeLine;
using Entitas;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Effect", "Remove Bind Type Effect", TimeLineExportPlatform.Client)]
    public class RemoveEffectByBindTypeEvent : AEventItem
    {
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int ConfigID { get; set; }

        public override void Trigger()
        {
            GameEntity skillEntity = GetGameEntity();
            IGroup<GameEntity> entityGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Effect, GameMatcher.BindNodeEffect));

#if TIMELINE_DEBUG
            int removeCount = 0;
#endif
            foreach (var entity in entityGroup.GetEntities())
            {
                if (entity.configID.value == ConfigID &&
                    entity.bindNodeEffect.nodeType == NodeType)
                {
                    entity.isMarkDestroy = true;
#if TIMELINE_DEBUG
                    ++removeCount;
#endif
                }
            }

#if TIMELINE_DEBUG
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(skillEntity.childOf.entityID);
            INodeBehaviourView nbView = ownerEntity.virtualView.value as INodeBehaviourView;
            int bindNodeCount = nbView.GetNodeBindCount(NodeType);
            if(bindNodeCount != removeCount)
            {
                services.logService.Log(DebugLogType.Error, $"RemoveEffectByBindTypeEvent::Trigger->the Count of removed is not equal to the count of bindNode. configId = {ConfigID},type = {NodeType}");
            }
            else
            {
                services.logService.Log(DebugLogType.Info, $"RemoveEffectByBindTypeEvent::Trigger->Remove Effect Success. configId = {ConfigID},type = {NodeType}");
            }
#endif
        }
    }
}
