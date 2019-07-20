namespace Dot.Core.TimeLine.Base.Condition
{
    [TimeLineMark("Condition","All Of",TimeLineExportPlatform.ALL)]
    public sealed class AllofCondition : AComposeCondition
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
