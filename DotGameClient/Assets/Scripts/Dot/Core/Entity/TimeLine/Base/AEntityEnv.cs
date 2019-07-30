using Dot.Core.Entity;

namespace Dot.Core.TimeLine
{
    public abstract class AEntityEnv
    {
        protected EntityContext contexts = null;
        protected EntityObject entity = null;

        protected bool isInit = false;

        public virtual void Initialize(EntityContext contexts, EntityObject entity)
        {
            if (isInit) return;

            this.contexts = contexts;
            this.entity = entity;
            isInit = true;
        }

        public virtual void DoReset()
        {
            contexts = null;
            entity = null;
            isInit = false;
        }
    }
}
