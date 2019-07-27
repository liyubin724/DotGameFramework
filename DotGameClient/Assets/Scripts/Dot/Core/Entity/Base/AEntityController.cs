using Dot.Core.Event;

namespace Dot.Core.Entity
{
    public abstract class AEntityController
    {
        protected EntityObject Entity { get; }
        public EventDispatcher Dispatcher { get; set; }
        public bool Enable { get; set; }

        public AEntityController(EntityObject entity)
        {
            Entity = entity;
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
        }
    }
}
