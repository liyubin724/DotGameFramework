using UnityEngine;
using System;

namespace Dot.Core.Entity.Data
{
    public enum TargetType
    {
        None,
        Position,
        Transform,
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

        private WeakReference<Transform> targetTransform = null;
        public void SetTargetTransform(Transform transform)
        {
            targetTransform = new WeakReference<Transform>(transform);
            preTargetPosition = transform.position;
            targetType = TargetType.Transform;
        }

        private Vector3 targetPosition = Vector3.zero;
        public void SetTargetPosition(Vector3 tPosition)
        {
            targetPosition = tPosition;
            targetType = TargetType.Position;
        }

        private long entityUniqueID = 0;
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

            if(targetType == TargetType.Transform)
            {
                if(targetTransform.TryGetTarget(out Transform target))
                {
                    preTargetPosition = target.position;
                }
                return preTargetPosition;
            }

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
