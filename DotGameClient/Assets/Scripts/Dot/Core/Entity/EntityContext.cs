using Dot.Core.Event;
using Dot.Core.Generic;
using Dot.Core.Logger;
using Dot.Core.Util;
using System.Collections.Generic;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Entity
{
    public class EntityContext
    {
        private Transform entityRootTran = null;
        private EventDispatcher eventDispatcher = new EventDispatcher();
        public Transform EntityRootTransfrom { get => entityRootTran; }

        private UniqueIDCreator idCreator = new UniqueIDCreator();

        private Dictionary<long, EntityObject> entityDic = new Dictionary<long, EntityObject>();
        private Dictionary<int, List<EntityObject>> entityCategroyDic = new Dictionary<int, List<EntityObject>>();

        private Dictionary<int, AEntityBuilder> entityCreatorDic = new Dictionary<int, AEntityBuilder>();
        public EntityContext()
        {
            entityRootTran = DontDestroyHandler.CreateTransform("Entity Root");
        }

        public void DoUpdate(float deltaTime)
        {
            foreach(var kvp in entityDic)
            {
                kvp.Value.DoUpdate(deltaTime);
            }
        }

        public void RegisterEntityCreator(int entityType, AEntityBuilder builder)
        {
            if(!entityCreatorDic.ContainsKey(entityType))
            {
                entityCreatorDic.Add(entityType, builder);
            }
        }

        public EntityObject CreateEntity(int entityType)
        {
            if(entityCreatorDic.TryGetValue(entityType,out AEntityBuilder builder))
            {
                EntityObject entity = builder.CreateEntityObject(idCreator.Next(),entityType);
                AddEntity(entity);
                return entity;
            }
            return null;
        }

        public void AddEntity(EntityObject entity)
        {
            if(entityDic.ContainsKey(entity.UniqueID))
            {
                DebugLogger.LogError("");
                return;
            }

            entityDic.Add(entity.UniqueID, entity);
            if(!entityCategroyDic.TryGetValue(entity.Category,out List<EntityObject> entities))
            {
                entities = new List<EntityObject>();
                entityCategroyDic.Add(entity.Category, entities);
            }
            entities.Add(entity);
        }

        public void DeleteEntity(EntityObject entity)
        {
            if(entityDic.ContainsKey(entity.UniqueID))
            {
                entityDic.Remove(entity.UniqueID);
            }
            if (entityCategroyDic.TryGetValue(entity.Category, out List<EntityObject> entities))
            {
                entities.Remove(entity);
            }

            if (entityCreatorDic.TryGetValue(entity.Category, out AEntityBuilder builder))
            {
                builder.DestroyEntityObject(entity);
            }
        }

        public void SendEvent(long receiverIndex,int eventID,params SystemObject[] datas)
        {
            if(entityDic.TryGetValue(receiverIndex,out EntityObject entityObject))
            {
                entityObject.SendEvent(eventID, datas);
            }
        }
    }


}
