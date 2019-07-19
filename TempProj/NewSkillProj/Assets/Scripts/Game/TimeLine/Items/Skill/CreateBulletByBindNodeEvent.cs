using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Skill", "Create Bullet By BindNode", TimeLineExportPlatform.ALL)]
    public class CreateBulletByBindNodeEvent : AEventItem
    {
        public int NodeIndex { get; set; } = 0;
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int ConfigID { get; set; }

        public override void Trigger()
        {
            GameEntity skillEntity = GetGameEntity();
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(skillEntity.childOf.entityID);
            INodeBehaviourView nbView = null;//ownerEntity.virtualView.view as INodeBehaviourView;
            //TODO:
            BindNodeData nodeData = nbView.GetNodeBindData(NodeType, NodeIndex);

#if TIMELINE_DEBUG
            if(nodeData == null)
            {
                services.logService.Log(DebugLogType.Error, $"CreateBulletEvent::Trigger->The bindNode not found. index = {NodeIndex},type = {NodeType}");
                return;
            }
#endif
            GameEntity bulletEntity = services.entityFactroy.CreateBulletEntity(skillEntity, ConfigID);
            bulletEntity.AddPosition(nodeData.nodeTransform.position);
            SkillConfigData scData = services.dataService.GetSkillData(skillEntity.configID.value);
            if(!ownerEntity.hasSelectedTarget)
            {
                bulletEntity.AddDirection(nodeData.nodeTransform.right);
            }else
            {
                GameEntity targetEntity = contexts.game.GetEntityWithUniqueID(ownerEntity.selectedTarget.entityID);
                if(scData.targetType == SkillTargetType.Position)
                {
                    bulletEntity.AddPositionTarget(targetEntity.position.value);
                    bulletEntity.AddDirection((targetEntity.position.value-nodeData.nodeTransform.position).normalized);
                }else if(scData.targetType == SkillTargetType.Entity)
                {
                    bulletEntity.AddEntityTarget(targetEntity.uniqueID.value);
                    bulletEntity.AddDirection((targetEntity.position.value - nodeData.nodeTransform.position).normalized);
                }else
                {
                    bulletEntity.AddDirection(nodeData.nodeTransform.right);
                }
            }



#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"CreateBulletEvent::Trigger->Create Bullet.configId = {ConfigID}, index = {NodeIndex},type = {NodeType}");
#endif
        }
    }
}
