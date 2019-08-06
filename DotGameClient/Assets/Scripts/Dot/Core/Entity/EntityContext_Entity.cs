using Dot.Core.Logger;
using System.Collections.Generic;

namespace Dot.Core.Entity
{
    public partial class EntityContext
    {
        private Dictionary<long, EntityObject> entityDic = new Dictionary<long, EntityObject>();
        private Dictionary<int, List<EntityObject>> entityCategoryDic = new Dictionary<int, List<EntityObject>>();

        private Dictionary<int, AEntityBuilder> entityBuilderDic = new Dictionary<int, AEntityBuilder>();

        public void RegisterEntityBuilder(int catetory, AEntityBuilder builder)
        {
            if (!entityBuilderDic.ContainsKey(catetory))
            {
                builder.Context = this;
                entityBuilderDic.Add(catetory, builder);
            }
        }

        public EntityObject CreateEntity(int catetory, int[] controllers)
        {
            if (entityBuilderDic.TryGetValue(catetory, out AEntityBuilder builder))
            {
                EntityObject entity = builder.CreateEntity(idCreator.Next(), catetory,controllers);
                AddEntity(entity);
                return entity;
            }
            return null;
        }

        public void DeleteEntity(EntityObject entity)
        {
            if (entityDic.ContainsKey(entity.UniqueID))
            {
                entityDic.Remove(entity.UniqueID);
            }

            if (entityCategoryDic.TryGetValue(entity.Category, out List<EntityObject> entities))
            {
                entities.Remove(entity);
            }

            if (entityBuilderDic.TryGetValue(entity.Category, out AEntityBuilder builder))
            {
                builder.DeleteEntity(entity);
            }
        }

        public EntityObject GetEntity(long uniqueID)
        {
            if(entityDic.TryGetValue(uniqueID,out EntityObject entity))
            {
                return entity;
            }
            return null;
        }

        public EntityObject[] GetCategoryEntity(int category)
        {
            if (entityCategoryDic.TryGetValue(category, out List<EntityObject> entities))
            {
                return entities.ToArray();
            }
            return null;
        }

        private void AddEntity(EntityObject entity)
        {
            if (entityDic.ContainsKey(entity.UniqueID))
            {
                DebugLogger.LogError("");
                return;
            }

            entityDic.Add(entity.UniqueID, entity);

            if (!entityCategoryDic.TryGetValue(entity.Category, out List<EntityObject> entities))
            {
                entities = new List<EntityObject>();
                entityCategoryDic.Add(entity.Category, entities);
            }
            entities.Add(entity);
        }
    }
}
