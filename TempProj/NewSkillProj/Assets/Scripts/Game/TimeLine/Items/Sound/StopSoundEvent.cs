﻿using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Sound", "Stop Sound", TimeLineExportPlatform.Client)]
    public class StopSoundEvent : AEventItem
    {
        [ItemDependOnAttribute(typeof(PlaySoundEvent))]
        public int SoundIndex { get; set; }

        public override void Trigger()
        {
            var entities = contexts.game.GetEntitiesWithChildOf((entity as GameEntity).uniqueID.value);
            foreach (var e in entities)
            {
                if (e.isSound && e.hasFCID && e.fCID.value == SoundIndex)
                {
                    e.isMarkDestroy = true;
                    //TODO:Stop Sound
#if DTL_DEBUG
                services.logService.Log(DebugLogType.Info, "DTLStopSoundEvent::DoEnter->Stop Sound");
#endif
                    break;
                }
            }
        }
    }
}