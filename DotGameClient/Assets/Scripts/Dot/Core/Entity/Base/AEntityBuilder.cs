using Dot.Core.Pool;
using System;
using System.Collections.Generic;

namespace Dot.Core.Entity
{
    public abstract class AEntityBuilder : IDisposable
    {
        private Dictionary<int, ObjectPool> controllerPoolDic = new Dictionary<int, ObjectPool>();
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
            if (!controllerPoolDic.TryGetValue(index, out ObjectPool pool))
            {
                pool = new ObjectPool();
                controllerPoolDic.Add(index, pool);
            }
            return pool.Get<T>();
        }

        protected void ReleaseController(int index,AEntityController controller)
        {
            if (controllerPoolDic.TryGetValue(index, out ObjectPool pool))
            {
                pool.Release(controller);
            }
        }
    }
}
