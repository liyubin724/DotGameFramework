using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Sound", "Play Sound", TimeLineExportPlatform.Client)]
    public class PlaySoundEvent : AEventItem
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
