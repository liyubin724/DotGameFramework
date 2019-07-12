using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Skill", "Create Bullet", TimeLineExportPlatform.ALL)]
    public class CreateBulletEvent : AEventItem
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
            if(nodeData == null)
            {
                services.logService.Log(DebugLogType.Error, $"CreateBulletEvent::Trigger->The bindNode not found. index = {NodeIndex},type = {NodeType}");
                return;
            }
#endif
            GameEntity bulletEntity = services.entityFactroy.CreateBulletEntity(skillEntity, ConfigID);
            bulletEntity.AddPosition(nodeData.nodeTransform.position);
            bulletEntity.AddDirection(nodeData.nodeTransform.right);

#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Error, $"CreateBulletEvent::Trigger->Create Bullet.configId = {ConfigID}, index = {NodeIndex},type = {NodeType}");
#endif
        }
    }
}
