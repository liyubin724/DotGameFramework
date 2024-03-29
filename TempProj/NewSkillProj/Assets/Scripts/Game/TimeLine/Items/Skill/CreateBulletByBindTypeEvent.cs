﻿using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Skill", "Create Bullet By BindType", TimeLineExportPlatform.ALL)]
    public class CreateBulletByBindTypeEvent : AEventItem
    {
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int ConfigID { get; set; }

        public override void Trigger()
        {
            GameEntity skillEntity = GetGameEntity();
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(skillEntity.parent.entityID);
            INodeBehaviourView nbView = ownerEntity.view.view as INodeBehaviourView;
            int bindNodeCount = nbView.GetNodeBindCount(NodeType);
            for (var i = 0; i < bindNodeCount; i++)
            {
                BindNodeData nodeData = nbView.GetNodeBindData(NodeType, i);
#if TIMELINE_DEBUG
                if (nodeData == null)
                {
                    services.logService.Log(DebugLogType.Error, $"CreateBulletByBindTypeEvent::Trigger->The bindNode not found. index = {i},type = {NodeType}");
                    continue;
                }
#endif
                GameEntity bulletEntity = services.entityFactroy.CreateBulletEntity(skillEntity, ConfigID);
                bulletEntity.AddPosition(nodeData.nodeTransform.position);

                SkillConfigData scData = services.dataService.GetSkillData(skillEntity.configID.value);
                if (!ownerEntity.hasSelectedTarget)
                {
                    bulletEntity.AddDirection(nodeData.nodeTransform.forward);
                }
                else
                {
                    GameEntity targetEntity = contexts.game.GetEntityWithUniqueID(ownerEntity.selectedTarget.entityID);
                    if (scData.targetType == SkillTargetType.Position)
                    {
                        bulletEntity.AddPositionTarget(targetEntity.position.value);
                        bulletEntity.AddDirection((targetEntity.position.value - nodeData.nodeTransform.position).normalized);
                    }
                    else if (scData.targetType == SkillTargetType.Entity)
                    {
                        bulletEntity.AddEntityTarget(targetEntity.uniqueID.value);
                        bulletEntity.AddDirection((targetEntity.position.value - nodeData.nodeTransform.position).normalized);
                    }
                    else
                    {
                        bulletEntity.AddDirection(nodeData.nodeTransform.forward);
                    }
                }
            }
#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"CreateBulletEvent::Trigger->Create Bullet.configId = {ConfigID},type = {NodeType}");
#endif
        }
    }
}
