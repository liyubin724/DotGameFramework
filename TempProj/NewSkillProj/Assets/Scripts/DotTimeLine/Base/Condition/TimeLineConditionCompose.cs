using System.Collections.Generic;
using Entitas;

namespace DotTimeLine.Base.Condition
{
    public class TimeLineConditionCompose : ATimeLineEnv
    {
        public readonly List<ATimeLineCondition> conditions = new List<ATimeLineCondition>();

        public override void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            base.Initialize(contexts, services, entity);
            conditions.ForEach((condition) =>
            {
                condition.Initialize(contexts, services, entity);
            });
        }

        public bool Evaluate()
        {
            bool result = true;
            foreach(var c in conditions)
            {
                if(!c.Evaluate())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public void DoUpdate(float deltaTime)
        {
            foreach(var c in conditions)
            {
                c.DoUpdate(deltaTime);
            }
        }
    }
}
