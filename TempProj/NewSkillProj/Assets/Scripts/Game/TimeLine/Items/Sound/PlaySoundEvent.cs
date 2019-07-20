using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Sound", "Play Sound", TimeLineExportPlatform.Client)]
    public class PlaySoundEvent : AEventItem
    {
        public int SoundConfigID { get; set; }
        public override void Trigger()
        {
            GameEntity soundEntity = services.entityFactroy.CreateSoundEntity(GetGameEntity(), SoundConfigID);
            soundEntity.AddTimeLineID(Index);

#if TIMELINE_DEBUG
        services.logService.Log(DebugLogType.Info, $"PlaySoundEvent::Trigger->Play Sound.soundID = {SoundConfigID}");
#endif
        }
    }
}
