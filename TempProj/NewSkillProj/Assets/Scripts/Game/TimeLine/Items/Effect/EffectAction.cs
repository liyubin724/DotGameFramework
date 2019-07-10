using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineItem("Effect", "Effect", TimeLineItemPlatform.Client)]
    public class EffectAction : ATimeLineActionItem
    {
        public int EffectConfigID { get; set; } = -1;

        [TimeLineDependOn(typeof(CreateEmitEvent))]
        public int EmitIndex { get; set; } = -1;

        private GameEntity effectEntity;
        public override void Enter()
        {
            GameEntity gameEntity = entity as GameEntity;
            effectEntity = services.entityFactroy.CreateEffectEntity(gameEntity, EffectConfigID);
            effectEntity.AddTimeLineID(Index);
            EffectView effectView = effectEntity.virtualView.value as EffectView;
            SkillEmitData emitData = gameEntity.skillEmit.dataDic[EmitIndex];

            effectView.RootTransform.SetParent(emitData.bindNodeData.nodeTransform, false);

#if DTL_DEBUG
        services.logService.Log(DebugLogType.Info, "DTLEffectAction::DoEnter->Added Effect");
#endif
        }

        public override void Exit()
        {
            effectEntity.isMarkDestroy = true;
            effectEntity = null;
        }

        public override void DoReset()
        {
            if(effectEntity!=null)
            {
                effectEntity.isMarkDestroy = true;
                effectEntity = null;
            }
            base.DoReset();
        }
    }
}
