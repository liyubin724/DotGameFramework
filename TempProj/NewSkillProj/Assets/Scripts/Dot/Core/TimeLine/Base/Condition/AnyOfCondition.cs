namespace Dot.Core.TimeLine.Base.Condition
{
    [TimeLineMark("Condition","Any Of",TimeLineExportPlatform.ALL)]
    public sealed class AnyOfCondition : AComposeCondition
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
