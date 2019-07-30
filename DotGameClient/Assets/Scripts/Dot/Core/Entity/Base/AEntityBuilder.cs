using System;
using System.Collections.Generic;

namespace Dot.Core.Entity
{
    public abstract class AEntityBuilder : IDisposable
    {
        private Dictionary<int, EntityControllerPool> controllerPoolDic = new Dictionary<int, EntityControllerPool>();

        public EntityContext Context { get; set; }

        public abstract EntityObject CreateEntityObject(long uniqueID,int entityType);
        public abstract void DestroyEntityObject(EntityObject entity);

        public virtual void Dispose()
        {
            
        }

        protected T AddControllerToEntity<T>(EntityObject entity,int index) where T:AEntityController,new()
        {
            T controller = GetOrCreateController<T>(index);
            controller.InitializeController(Context, entity);
            entity.AddController(index, controller);

            return controller;
        }

        protected T GetOrCreateController<T>(int index) where T : AEntityController,new()
        {
            if(!controllerPoolDic.TryGetValue(index,out EntityControllerPool pool))
            {
                pool = new EntityControllerPool(10);
                controllerPoolDic.Add(index,pool);
            }
            return (T)pool.Get();
        }

        protected void ReleaseController(int index,AEntityController controller)
        {
            if (controllerPoolDic.TryGetValue(index, out EntityControllerPool pool))
            {
                pool.Release(controller);
            }
        }
    }
}
