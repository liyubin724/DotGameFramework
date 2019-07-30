namespace Dot.Core.Entity
{
    public class EntityViewController : AEntityController
    {
        private AEntityView entityView;
        
        public void SetView(AEntityView view)
        {
            entityView = view;
            entityView.InitializeView(entity);
        }

        public T GetView<T>() where T:AEntityView
        {
            if(entityView !=null)
            {
                return (T)entityView;
            }
            return null;
        }

        protected override void AddEventListeners()
        {
            
        }

        protected override void RemoveEventListeners()
        {
            
        }

        public override void DoReset()
        {
            base.DoReset();
        }
    }
}
