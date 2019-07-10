using DotTimeLine.Base.Items;

namespace Game.TimeLine
{
    [TimeLineItem("Sound", "Play Sound", TimeLineItemPlatform.Client)]
    public class PlaySoundEvent : ATimeLineEventItem
    {
        public int SoundConfigID { get; set; }
        public override void Trigger()
        {
            GameEntity soundEntity = services.entityFactroy.CreateSoundEntity((entity as GameEntity), SoundConfigID);
            soundEntity.AddTimeLineID(Index);
#if DTL_DEBUG
        services.logService.Log(DebugLogType.Info, "DTLPlaySoundEvent::DoEnter->Play Sound");
#endif
        }
    }
}
