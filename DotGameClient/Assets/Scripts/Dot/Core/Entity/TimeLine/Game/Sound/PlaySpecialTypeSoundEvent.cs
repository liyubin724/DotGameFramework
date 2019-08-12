using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Sound", "Special Type", TimeLineExportPlatform.Client)]
    public class PlaySpecialTypeSoundEvent : AEventItem
    {
        public int MusicID { get; set; }
#if !NOT_ETERNITY
        public WwiseMusicSpecialType SpecialType { get; set; }
        public WwiseMusicPalce Place{get;set;}
#endif
        public override void DoRevert()
        {
           
        }

        public override void Trigger()
        {
#if !NOT_ETERNITY
            WwiseUtil.PlaySound(MusicID,SpecialType,Place,false,null);
#endif
        }
    }
}
