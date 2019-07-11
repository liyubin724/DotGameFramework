namespace Dot.Core.TimeLine.Base.Condition
{
    [TimeLineMark("Condition/Time Over","Time Over",TimeLineExportPlatform.ALL)]
    public class TimeOverCondition : ACondition
    {
        public float TotalTime { get; set; }
        private float elapsedTime=0.0f;
        public override bool Evaluate()
        {
            if(elapsedTime>=TotalTime)
            {
                return true;
            }
            return false;
        }

        public override void DoUpdate(float deltaTime)
        {
            if(elapsedTime<TotalTime)
            {
                elapsedTime += deltaTime;
            }
        }

        public override void DoReset()
        {
            elapsedTime = 0.0f;
            base.DoReset();
        }
    }
}
