using Entitas;

namespace Dot.Core.TimeLine
{
    public abstract class AEntitasEnv
    {
        protected Contexts contexts = null;
        protected Services services = null;
        protected IEntity entity = null;
        protected bool isInit = false;
        protected GameEntity GetGameEntity()
        {
            return (GameEntity)entity;
        }

        public virtual void Initialize(Contexts contexts, Services services, IEntity entity)
        {
            this.contexts = contexts;
            this.services = services;
            this.entity = entity;
            isInit = true;
        }

        public virtual void DoReset()
        {
            contexts = null;
            services = null;
            entity = null;
            isInit = false;
        }
    }
}
