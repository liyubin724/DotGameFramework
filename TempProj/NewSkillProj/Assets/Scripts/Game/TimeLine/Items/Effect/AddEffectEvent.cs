using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Effect","Add Effect",TimeLineExportPlatform.Client)]
    public class AddEffectEvent : AEventItem
    {
        public int EffectConfigID { get; set; } = -1;
        [ItemDependOnAttribute(typeof(CreateEmitEvent))]
        public int EmitIndex { get; set; } = -1;

        public override void Trigger()
        {
            GameEntity gameEntity = entity as GameEntity;
            GameEntity effectEntity = services.entityFactroy.CreateEffectEntity(gameEntity, EffectConfigID);
            effectEntity.AddTimeLineID(Index);
            EffectView effectView = effectEntity.virtualView.value as EffectView;
            SkillEmitData emitData = gameEntity.skillEmit.dataDic[EmitIndex];

            effectView.RootTransform.SetParent(emitData.bindNodeData.nodeTransform, false);

#if DTL_DEBUG
        services.logService.Log(DebugLogType.Info, "DTLAddEffectEvent::DoEnter->Added Effect");
#endif
        }

        public override void SetDefaults()
        {
            EffectConfigID = -1;
            EmitIndex = -1;
        }
    }
}
