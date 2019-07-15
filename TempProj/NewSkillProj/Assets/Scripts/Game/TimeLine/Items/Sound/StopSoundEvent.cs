using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;
using Entitas;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Sound", "Stop Sound", TimeLineExportPlatform.Client)]
    public class StopSoundEvent : AEventItem
    {
        [ItemDependOn(typeof(PlaySoundEvent))]
        public int SoundIndex { get; set; }

        public override void Trigger()
        {
            IGroup<GameEntity> soundGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Sound, GameMatcher.TimeLineID));
            foreach(var e in soundGroup.GetEntities())
            {
                if(e.timeLineID.value == SoundIndex)
                {
                    e.isMarkDestroy = true;
                    break;
                }
            }
        }
    }
}
