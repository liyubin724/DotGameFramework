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
        public virtual void DestroyEntityObject(EntityObject entity)
        {
            if (entity == null) return;

            entity.RemoveAllController(out int[] indexes, out AEntityController[] controllers);

            if (indexes != null && indexes.Length > 0)
            {
                for (int i = 0; i < indexes.Length; ++i)
                {
                    ReleaseController(indexes[i], controllers[i]);
                }
            }
        }

        public virtual void Dispose()
        {
            
        }

        protected EntityObject CreateEntity(long uniqueID, int category)
        {
            EntityObject entity = new EntityObject();
            entity.UniqueID = uniqueID;
            entity.Category = category;
            entity.Name = $"{EntityCategroyConst.GetCategroyName(category)}_{category}_{uniqueID}";

            return entity;
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
