using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMark("Event/Motion", "Set Motion Curve", TimeLineExportPlatform.ALL)]
    public class SetMotionCurveTypeEvent : AEventItem
    {
        public MotionCurveType Motion { get; set; }

        private MotionCurveType cachedMotion = MotionCurveType.None;
        private bool hasMotion = false;
        public override void Trigger()
        {
            hasMotion = GetGameEntity().hasMotionCurveType;
            if(hasMotion)
            {
                cachedMotion = GetGameEntity().motionCurveType.value;
            }

            GetGameEntity().ReplaceMotionCurveType(Motion);
        }

        public override void DoRevert()
        {
            if(!hasMotion)
            {
                GetGameEntity().RemoveMotionCurveType();
            }else
            {
                GetGameEntity().ReplaceMotionCurveType(cachedMotion);
            }
        }

        public override void DoReset()
        {
            cachedMotion = MotionCurveType.None;
            hasMotion = false;
            base.DoReset();
        }
    }
}
