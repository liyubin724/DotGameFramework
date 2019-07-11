﻿namespace Dot.Core.TimeLine.Base.Condition
{
    public abstract class ATimeLineCondition : ATimeLineEnv
    {
        public bool IsReadonly { get; set; }

        public virtual void DoUpdate(float deltaTime)
        {
        }
        public abstract bool Evaluate();
    }
}
