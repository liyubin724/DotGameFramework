using Dot.Core.Entity.Data;
using Game.Entity;
using System;

namespace Dot.Core.Entity
{
    public abstract class AEntityBuilder
    {
        public EntityContext Context { get; set; }

        public EntityObject CreateEntity(long uniqueID, int category,int[] controllerIndexes)
        {
            EntityObject entity = new EntityObject();
            entity.UniqueID = uniqueID;
            entity.Category = category;
            entity.Name = $"{EntityCategroyConst.GetCategroyName(category)}_{category}_{uniqueID}";

            if(controllerIndexes!=null)
            {
                AEntityController[] controllers = new AEntityController[controllerIndexes.Length];
                for(int i =0;i<controllerIndexes.Length;++i)
                {
                    AEntityController controller = EntityControllerFactory.GetController(category,controllerIndexes[i]);
                    entity.AddController(controllerIndexes[i], controller);

                    controllers[i] = controller;
                }

                Array.ForEach(controllers, (controller) => {
                    controller.InitializeController(Context, entity);
                });
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
