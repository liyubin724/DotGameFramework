using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Effect", "Add Effect With Owner", TimeLineExportPlatform.Client)]
    public class AddEffectWithOwnerAction : AActionItem
    {
        public int ConfigID { get; set; }

        private GameEntity effectEntity = null;
        public override void Enter()
        {
            GameEntity entity = GetGameEntity();
            effectEntity = services.entityFactroy.CreateEffectEntity(GetGameEntity(), ConfigID);
            effectEntity.AddPosition(entity.position.value);
        }

        public override void Exit()
        {
            effectEntity.isMarkDestroy = true;
        }

        public override void Stop()
        {
            effectEntity.isMarkDestroy = true;
        }
    }
}
