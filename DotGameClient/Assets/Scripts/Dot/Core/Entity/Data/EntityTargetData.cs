using UnityEngine;

namespace Dot.Core.Entity.Data
{
    public enum TargetType
    {
        None,
        Position,
        Entity,
    }

    public interface ITargetData
    {
        EntityTargetData GetTargetData();
    }

    public class EntityTargetData
    {
        private TargetType targetType = TargetType.None;
        private Vector3 preTargetPosition = Vector3.zero;

        private Vector3 targetPosition = Vector3.zero;
        public void SetTargetPosition(Vector3 tPosition)
        {
            targetPosition = tPosition;
            targetType = TargetType.Position;
        }

        private long entityUniqueID = 0;
        public long GetEntityUniqueID() => entityUniqueID;
        public void SetEntityUniqueID(long id)
        {
            entityUniqueID = id;
            targetType = TargetType.Entity;
            EntityContext context = EntityContext.GetInstance();
            if(context!=null)
            {
                EntityObject entity = context.GetEntity(entityUniqueID);
                if(entity!=null && entity.EntityData!=null)
                {
                    preTargetPosition = entity.EntityData.GetPosition();
                }
            }
        }

        public bool HasTarget() => targetType != TargetType.None;

        public Vector3 GetPosition()
        {
            if (targetType == TargetType.None)
                return Vector3.zero;

            if(targetType == TargetType.Position)
                return targetPosition;

            if(targetType == TargetType.Entity)
            {
                EntityContext context = EntityContext.GetInstance();
                if (context != null)
                {
                    EntityObject entity = context.GetEntity(entityUniqueID);
                    if (entity != null && entity.EntityData != null)
                    {
                        preTargetPosition = entity.EntityData.GetPosition();
                    }
                }
                return preTargetPosition;
            }

            return Vector3.zero;
        }
    }
}
