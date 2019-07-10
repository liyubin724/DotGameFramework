using Entitas;

namespace Dot.Core.TimeLine.Base
{
    public abstract class ATimeLineEnv
    {
        protected Contexts contexts = null;
        protected Services services = null;
        protected IEntity entity = null;

        public virtual void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            this.contexts = contexts;
            this.services = services;
            this.entity = entity;
        }

        public virtual void DoReset()
        {
            contexts = null;
            services = null;
            entity = null;
        }
    }
}
