using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Set Acceleration", TimeLineExportPlatform.ALL)]
    public class SetAccelerationEvent : AEventItem
    {
        public float Acceleration { get; set; }

        private float cachedAcc;
        private bool hasAcc;
        public override void Trigger()
        {
            hasAcc = GetGameEntity().hasAcceleration;
            if(hasAcc)
            {
                cachedAcc = GetGameEntity().acceleration.value;
            }
            GetGameEntity().ReplaceAcceleration(Acceleration);
#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"ChangeAccelerationEvent::Trigger->Changed Acc.value = {Acceleration}");
#endif
        }

        public override void DoRevert()
        {
            if(!hasAcc)
            {
                GetGameEntity().RemoveAcceleration();
            }else
            {
                GetGameEntity().ReplaceAcceleration(cachedAcc);
            }
        }

        public override void DoReset()
        {
            cachedAcc = 0.0f;
            hasAcc = false;
            base.DoReset();
        }
    }
}
