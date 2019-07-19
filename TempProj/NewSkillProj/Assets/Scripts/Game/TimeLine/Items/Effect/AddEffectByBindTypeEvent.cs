using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Effect", "Add Bind Type Effect", TimeLineExportPlatform.Client)]
    public class AddEffectByBindTypeEvent : AEventItem
    {
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int ConfigID { get; set; }

        public override void Trigger()
        {
            GameEntity skillEntity = GetGameEntity();
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(skillEntity.parent.entityID);
            INodeBehaviourView nbView = ownerEntity.view.view as INodeBehaviourView;
            int bindNodeCount = nbView.GetNodeBindCount(NodeType);
            for(var i =0;i<bindNodeCount;i++)
            {
                BindNodeData nodeData = nbView.GetNodeBindData(NodeType, i);
#if TIMELINE_DEBUG
                if (nodeData == null)
                {
                    services.logService.Log(DebugLogType.Error, $"AddEffectByBindTypeEvent::Trigger->The bindNode not found. index = {i},type = {NodeType}");
                    continue;
                }
#endif

                GameEntity effectEntity = services.entityFactroy.CreateEffectEntity(skillEntity, ConfigID);
                effectEntity.AddTimeLineID(Index);
                effectEntity.AddBindNodeEffect(i, NodeType);
                EffectView effectView = null;//effectEntity.virtualView.view as EffectView;
                effectView.RootTransform.SetParent(nodeData.nodeTransform, false);
            }

#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"AddEffectByBindTypeEvent::Trigger->Create Effect.configId = {ConfigID},type = {NodeType}");
#endif
        }
    }
}
