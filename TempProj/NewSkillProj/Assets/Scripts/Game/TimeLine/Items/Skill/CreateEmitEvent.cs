using Dot.Core.TimeLine.Base.Item;
using System.Collections.Generic;

namespace Game.TimeLine
{
    [TimeLineItem("Skill", "Create Emit", TimeLineItemPlatform.ALL)]
    public class CreateEmitEvent : ATimeLineEventItem
    {
        public BindNodeType NodeType { get; set; } = BindNodeType.Main;
        public int NodeIndex { get; set; } = 0;

        public override void Trigger()
        {
            GameEntity gameEntity = entity as GameEntity;
            GameEntity ownerEntity = contexts.game.GetEntityWithUniqueID(gameEntity.childOf.entityID);
            INodeBehaviourView nbView = ownerEntity.virtualView.value as INodeBehaviourView;

            Dictionary<int, SkillEmitData> emitDataDic = null;
            if (gameEntity.hasSkillEmit)
            {
                emitDataDic = gameEntity.skillEmit.dataDic;
            }
            else
            {
                emitDataDic = new Dictionary<int, SkillEmitData>();
            }

            if (!emitDataDic.ContainsKey(Index))
            {
                SkillEmitData d = new SkillEmitData()
                {
                    id = Index,
                    nodeIndex = NodeIndex,
                    bindNodeData = nbView.GetNodeBindData(NodeType, NodeIndex),
                };
                emitDataDic.Add(Index, d);

                gameEntity.ReplaceSkillEmit(emitDataDic);

#if DTL_DEBUG
            services.logService.Log(DebugLogType.Info, "DTLCreateEmitEvent::DoEnter->Create Emit");
#endif
            }
        }
    }
}
