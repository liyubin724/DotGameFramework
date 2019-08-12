using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game.Sound
{
    [TimeLineMark("Event/Sound", "Nomal", TimeLineExportPlatform.Client)]
    public class PlaySoundEvent : AEventItem
    {
        public int MusicID { get; set; }

        public override void DoRevert()
        {
        }

        public override void Trigger()
        {
#if !NOT_ETERNITY
            WwiseUtil.PlaySound(MusicID,false,null);
#endif
        }
    }
}
