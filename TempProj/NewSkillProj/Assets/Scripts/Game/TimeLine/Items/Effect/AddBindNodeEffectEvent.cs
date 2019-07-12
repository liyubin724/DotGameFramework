﻿using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Effect","Add Bind Node Effect",TimeLineExportPlatform.Client)]
    public class AddBindNodeEffectEvent : AEventItem
    {
        public int NodeIndex { get; set; } = 0;
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int ConfigID { get; set; }

        public override void Trigger()
        {
            GameEntity skillEntity = GetGameEntity();
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(skillEntity.childOf.entityID);
            INodeBehaviourView nbView = ownerEntity.virtualView.value as INodeBehaviourView;

            BindNodeData nodeData = nbView.GetNodeBindData(NodeType, NodeIndex);

#if TIMELINE_DEBUG
            if (nodeData == null)
            {
                services.logService.Log(DebugLogType.Error, $"AddBindNodeEffectEvent::Trigger->The bindNode not found. index = {NodeIndex},type = {NodeType}");
                return;
            }
#endif
            GameEntity effectEntity = services.entityFactroy.CreateEffectEntity(skillEntity, ConfigID);
            effectEntity.AddBindNodeEffect(NodeIndex, NodeType);
            EffectView effectView = effectEntity.virtualView.value as EffectView;
            effectView.RootTransform.SetParent(nodeData.nodeTransform, false);

#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Error, $"AddBindNodeEffectEvent::Trigger->Create Effect.configId = {ConfigID}, index = {NodeIndex},type = {NodeType}");
#endif
        }
    }
}
