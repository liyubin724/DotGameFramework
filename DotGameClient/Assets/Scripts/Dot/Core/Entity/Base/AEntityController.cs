using Dot.Core.Event;

namespace Dot.Core.Entity
{
    public abstract class AEntityController
    {
        protected EntityObject entity;
        protected EntityContext context;

        public EventDispatcher Dispatcher { get; set; }
        public bool Enable { get; set; }

        public void InitializeController(EntityContext context, EntityObject entityObj)
        {
            this.context = context;
            entity = entityObj;

            DoInit();
        }

        protected virtual void DoInit()
        {
            AddEventListeners();
        }

        public virtual void DoUpdate(float deltaTime) { }

        protected abstract void AddEventListeners();
        protected abstract void RemoveEventListeners();

        public virtual void DoReset()
        {
            RemoveEventListeners();
            context = null;
            entity = null;
            Dispatcher = null;
            Enable = true;
        }

        public virtual void DoDestroy()
        {
            DoReset();
        }
    }
}
