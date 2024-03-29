﻿using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Effect","Add Bind Node Effect",TimeLineExportPlatform.Client)]
    public class AddEffectByBindNodeEvent : AEventItem
    {
        public int NodeIndex { get; set; } = 0;
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int ConfigID { get; set; }

        public override void Trigger()
        {
            GameEntity skillEntity = GetGameEntity();
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(skillEntity.parent.entityID);
            INodeBehaviourView nbView = ownerEntity.view.view as INodeBehaviourView;
            BindNodeData nodeData = nbView.GetNodeBindData(NodeType, NodeIndex);

#if TIMELINE_DEBUG
            if (nodeData == null)
            {
                services.logService.Log(DebugLogType.Error, $"AddBindNodeEffectEvent::Trigger->The bindNode not found. index = {NodeIndex},type = {NodeType}");
                return;
            }
#endif
            GameEntity effectEntity = services.entityFactroy.CreateEffectEntity(skillEntity, ConfigID);
            effectEntity.AddTimeLineID(Index);
            effectEntity.AddBindNodeEffect(NodeIndex, NodeType);
            EffectView effectView = effectEntity.view.view as EffectView;
            effectView.RootTransform.SetParent(nodeData.nodeTransform, false);

#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Error, $"AddBindNodeEffectEvent::Trigger->Create Effect.configId = {ConfigID}, index = {NodeIndex},type = {NodeType}");
#endif
        }
    }
}
