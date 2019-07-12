namespace Dot.Core.TimeLine.Base.Condition
{
    [TimeLineMark("Condition","Parallel",TimeLineExportPlatform.ALL)]
    public sealed class ParallelCondition : AComposeCondition
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
