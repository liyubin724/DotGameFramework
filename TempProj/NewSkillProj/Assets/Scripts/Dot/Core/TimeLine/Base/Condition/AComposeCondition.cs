using System.Collections.Generic;
using Entitas;

namespace Dot.Core.TimeLine.Base.Condition
{
    public abstract class AComposeCondition : ACondition
    {
        public readonly List<ACondition> conditions = new List<ACondition>();

        public override void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            base.Initialize(contexts, services, entity);
            conditions.ForEach((condition) =>
            {
                condition.Initialize(contexts, services, entity);
            });
        }

        public override void DoUpdate(float deltaTime)
        {
            foreach(var c in conditions)
            {
                c.DoUpdate(deltaTime);
            }
        }

        public override void DoReset()
        {
            foreach(var c in conditions)
            {
                c.DoReset();
            }
            base.DoReset();
        }
    }
}
