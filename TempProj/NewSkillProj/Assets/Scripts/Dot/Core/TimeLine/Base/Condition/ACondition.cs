namespace Dot.Core.TimeLine.Base.Condition
{
    public abstract class ACondition : AEntitasEnv
    {
        public bool IsReadonly { get; set; }

        public virtual void DoUpdate(float deltaTime)
        {
        }
        public abstract bool Evaluate();
    }
}
