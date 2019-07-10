using DotTimeLine.Base.Items;

namespace Game.TimeLine
{
    [TimeLineItem("Sound", "Sound", TimeLineItemPlatform.Client)]
    public class SoundAction : ATimeLineActionItem
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
