using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

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
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(skillEntity.childOf.entityID);
            INodeBehaviourView nbView = ownerEntity.virtualView.value as INodeBehaviourView;
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
                bulletEntity.AddDirection(nodeData.nodeTransform.forward);
            }
#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"CreateBulletEvent::Trigger->Create Bullet.configId = {ConfigID},type = {NodeType}");
#endif
        }
    }
}
