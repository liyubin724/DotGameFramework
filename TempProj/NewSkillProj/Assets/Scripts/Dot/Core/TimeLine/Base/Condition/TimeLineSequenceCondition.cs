namespace Dot.Core.TimeLine.Base.Condition
{
    [TimeLineMark("Condition/Sequence","Sequence",TimeLineExportPlatform.ALL)]
    public class TimeLineSequenceCondition : ATimeLineComposeCondition
    {
        public override bool Evaluate()
        {
            foreach(var c in conditions)
            {
                if(!c.Evaluate())
                {
                    return false;
                }
            }
            return true;
        }
    }
}
