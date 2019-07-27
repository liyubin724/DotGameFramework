namespace Dot.Core.Entity
{
    public abstract class AEntityView
    {
        protected EntityObject entity = null;
        public virtual void InitializeView(EntityObject entityObj)
        {
            entity = entityObj;
            AddListener();
        }

        public virtual void DestroyView()
        {
            RemoveListener();
        }

       public abstract bool Active { get; set; }
        public abstract void AddListener();
        public abstract void RemoveListener();

    }
}
