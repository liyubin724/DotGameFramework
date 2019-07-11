namespace Dot.Core.TimeLine.Base.Condition
{
    [TimeLineMark("Condition/Parallel","Parallel",TimeLineExportPlatform.ALL)]
    public class TimeLineParallelCondition : ATimeLineComposeCondition
    {
        public override bool Evaluate()
        {
            foreach(var c in conditions)
            {
                if(c.Evaluate())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
