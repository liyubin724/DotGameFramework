using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Skill", "Create Bullet", TimeLineExportPlatform.ALL)]
    public class CreateBulletEvent : ATimeLineEventItem
    {
        public int BulletConfigID { get; set; }
        [TimeLineDependOn(typeof(CreateEmitEvent))]
        public int EmitIndex { get; set; }

        public override void Trigger()
        {
            GameEntity gameEntity = entity as GameEntity;
            SkillEmitData data = gameEntity.skillEmit.dataDic[EmitIndex];

            GameEntity bulletEntity = services.entityFactroy.CreateBulletEntity(gameEntity, BulletConfigID);
            bulletEntity.AddPosition(data.bindNodeData.nodeTransform.position);
            bulletEntity.AddDirection(data.bindNodeData.nodeTransform.right);

#if DTL_DEBUG
        services.logService.Log(DebugLogType.Info, "DTLCreateBulletEvent::DoEnter->emit bullet");
#endif
        }
    }
}
