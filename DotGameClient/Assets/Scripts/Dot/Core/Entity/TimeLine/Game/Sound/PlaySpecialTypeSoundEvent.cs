using Dot.Core.TimeLine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Sound", "Special Type", TimeLineExportPlatform.Client)]
    public class PlaySpecialTypeSoundEvent : AEventItem
    {
        public int MusicID { get; set; }
        //public WwiseMusicSpecialType SpecialType { get; set; }
        //public WwiseMusicPalce Place{get;set;}

        public override void DoRevert()
        {
           
        }

        public override void Trigger()
        {
            //WwiseUtil.PlaySound(MusicID,SpecialType,Place,false,null);
        }
    }
}
