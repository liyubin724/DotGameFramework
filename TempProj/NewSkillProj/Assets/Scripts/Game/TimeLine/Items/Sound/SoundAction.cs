using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Action/Sound", "Sound", TimeLineExportPlatform.Client)]
    public class SoundAction : AActionItem
    {
        public int SoundConfigID { get; set; }

        private GameEntity soundEntity = null;
        public override void Enter()
        {
            soundEntity = services.entityFactroy.CreateSoundEntity((entity as GameEntity), SoundConfigID);
            soundEntity.AddTimeLineID(Index);
#if DTL_DEBUG
        services.logService.Log(DebugLogType.Info, "DTLSoundAction::DoEnter->Play Sound");
#endif
        }

        public override void Exit()
        {
            soundEntity.isMarkDestroy = true;
        }
    }
}
