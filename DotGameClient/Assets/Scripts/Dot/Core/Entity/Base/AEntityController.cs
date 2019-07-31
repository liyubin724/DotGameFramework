using Dot.Core.Event;
using Dot.Core.Pool;

namespace Dot.Core.Entity
{
    public class AEntityController : IObjectPoolItem
    {
        public bool Enable { get; set; }

        protected EntityObject entity;
        protected EntityContext context;
        protected EventDispatcher Dispatcher { get => entity.Dispatcher; }

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
        protected virtual void AddEventListeners() { }
        protected virtual void RemoveEventListeners() { }

        public virtual void DoReset()
        {
            RemoveEventListeners();
            context = null;
            entity = null;
            Enable = true;
        }

        public void OnNew()
        {
        }

        public void OnRelease()
        {
            DoReset();
        }
    }
}
