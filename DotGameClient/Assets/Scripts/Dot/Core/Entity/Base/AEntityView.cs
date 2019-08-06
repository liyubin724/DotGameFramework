using Dot.Core.Event;

namespace Dot.Core.Entity
{
    public abstract class AEntityView
    {
        protected EntityObject entity = null;

        public void InitializeView(EntityObject entityObj)
        {
            entity = entityObj;
            DoInit();
        }

        protected virtual void DoInit()
        {
            AddListener();
        }
        
        public virtual void DoReset()
        {
            RemoveListener();
        }

        public virtual void DoDestroy()
        {
            DoReset();
        }

        public abstract bool Enable { get; set; }
        public abstract void AddListener();
        public abstract void RemoveListener();

    }
}
