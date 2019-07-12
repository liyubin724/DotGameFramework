﻿using Dot.Core.TimeLine.Base;
using Dot.Core.TimeLine.Base.Item;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Change Motion", TimeLineExportPlatform.ALL)]
    public class ChangeMotionTypeEvent : AEventItem
    {
        public MotionCurveType Motion { get; set; }
        public override void Trigger()
        {
            (entity as GameEntity).ReplaceMotionCurveType(Motion);
        }
    }
}
