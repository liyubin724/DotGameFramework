using Game.Entity;

namespace Dot.Core.Entity
{
    public abstract class AEntityBuilder
    {
        public EntityContext Context { get; set; }

        public EntityObject CreateEntity(long uniqueID, int category,int[] controllers)
        {
            EntityObject entity = new EntityObject();
            entity.UniqueID = uniqueID;
            entity.Category = category;
            entity.Name = $"{EntityCategroyConst.GetCategroyName(category)}_{category}_{uniqueID}";

            if(controllers!=null)
            {
                foreach(var index in controllers)
                {
                    AEntityController controller = EntityControllerFactory.GetController(index);
                    controller.InitializeController(Context, entity);

                    entity.AddController(index, controller);
                }
            }

            OnCreate(entity);

            return entity;
        }
        
        public void DeleteEntity(EntityObject entity)
        {

        }

        protected abstract void OnCreate(EntityObject entity);
        protected abstract void OnDelete(EntityObject entity);

    }
}
