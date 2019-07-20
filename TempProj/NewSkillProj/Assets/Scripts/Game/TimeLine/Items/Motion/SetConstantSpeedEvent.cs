using Dot.Core.TimeLine;

namespace Game.TimeLine
{
    [TimeLineMarkAttribute("Event/Motion", "Set Constant Speed", TimeLineExportPlatform.ALL)]
    public class SetConstantSpeedEvent : AEventItem
    {
        public float Speed { get; set; }

        private float cachedSpeed = 0.0f;
        private bool hasSpeed = false;
        public override void Trigger()
        {
            hasSpeed = GetGameEntity().hasSpeed;
            if(hasSpeed)
            {
                cachedSpeed = GetGameEntity().speed.value;
            }

            GetGameEntity().ReplaceSpeed(Speed);

#if TIMELINE_DEBUG
            services.logService.Log(DebugLogType.Info, $"ChangeSpeedEvent::Trigger->Change speed.value = {Speed}");
#endif
        }

        public override void DoRevert()
        {
            if(!hasSpeed)
            {
                GetGameEntity().RemoveSpeed();
            }else
            {
                GetGameEntity().ReplaceSpeed(cachedSpeed);
            }
        }

        public override void DoReset()
        {
            cachedSpeed = 0.0f;
            hasSpeed = false;
            base.DoReset();
        }
    }
}
