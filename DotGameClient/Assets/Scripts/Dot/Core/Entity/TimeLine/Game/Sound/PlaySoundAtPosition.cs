using Dot.Core.TimeLine;
using UnityEngine;

namespace Dot.Core.Entity.TimeLine.Game
{
    [TimeLineMark("Event/Sound", "At Position", TimeLineExportPlatform.Client)]
    public class PlaySoundAtPosition : AEventItem
    {
        public int MusicID { get; set; }
        public Vector3 Position { get; set; }
        public bool UseEntityPosition { get; set; } = false;

        public override void DoRevert()
        {
            
        }

        public override void Trigger()
        {
            Vector3 position = Position;
            if(UseEntityPosition)
            {
                position = entity.EntityData.GetPosition();
            }
#if !NOT_ETERNITY
            WwiseUtil.PlaySound(MusicID,false,position);
#endif
        }
    }
}
