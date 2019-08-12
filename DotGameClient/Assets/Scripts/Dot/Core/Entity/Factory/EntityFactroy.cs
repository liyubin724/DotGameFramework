using Dot.Config;
using Dot.Core.Entity.Controller;
using Dot.Core.Entity.Data;
using UnityEngine;

namespace Dot.Core.Entity
{
    public static class EntityFactroy
    {
        public static EntityObject CreateBullet(EntityObject creatorEntity,BindNodeData nodeData,int bulletConfigID,bool isUsedCreatorSpeed)
        {
            EntityObject bulletEntity = CreateDefaultBullet(bulletConfigID);
            bulletEntity.EntityData.OwnerUniqueID = creatorEntity.UniqueID;

            bulletEntity.EntityData.SetPosition(nodeData.transform.position);
            bulletEntity.EntityData.SetDirection(nodeData.transform.forward);

            BulletConfigData bulletConfigData = ConfigManager.GetInstance().GetBulletConfig(bulletConfigID);
            BulletEntityData bulletEntityData = bulletEntity.EntityData as BulletEntityData;
            if(isUsedCreatorSpeed)
            {
                EntityMoveData creatorMoveData = (creatorEntity.EntityData as IMoveData).GetMoveData();

                bulletEntityData.GetMoveData().SetOriginSpeed(creatorMoveData.GetSpeed());
            }
            
            if(bulletConfigData.targetType != TargetType.None)
            {
                EntityTargetData creatorTargetData = (creatorEntity.EntityData as ITargetData).GetTargetData();
                if (bulletConfigData.targetType == TargetType.Position)
                {
                    bulletEntityData.GetTargetData().SetTargetPosition(creatorTargetData.GetPosition());
                }else if(bulletConfigData.targetType == TargetType.Entity)
                {
                    bulletEntityData.GetTargetData().SetEntityUniqueID(creatorTargetData.GetEntityUniqueID());
                }
            }

            return bulletEntity;
        }

        public static EntityObject CreateDefaultBullet(int configID)
        {
            EntityObject bulletEntity = EntityContext.GetInstance().CreateEntity(
                EntityCategroyConst.BULLET,
                new int[]
                {
                    EntityControllerConst.SKELETON_INDEX,
                    EntityControllerConst.VIEW_INDEX,
                    EntityControllerConst.MOVE_INDEX,
                    EntityControllerConst.PHYSICS_INDEX,
                    EntityControllerConst.TIMELINE_INDEX,
                });
            bulletEntity.EntityData.ConfigID = configID;

            BulletConfigData bulletConfigData = ConfigManager.GetInstance().GetBulletConfig(configID);
            if(bulletConfigData.hasPhysics)
            {
                EntityViewController viewController = bulletEntity.GetController<EntityViewController>(EntityControllerConst.VIEW_INDEX);
                PhysicsVirtualView view = viewController.GetView<PhysicsVirtualView>();

                CapsuleCollider collider = view.GetOrCreateCollider(ColliderType.Capsule) as CapsuleCollider;
                collider.center = bulletConfigData.colliderCenter;
                collider.radius = bulletConfigData.colliderRadius;
                collider.height = bulletConfigData.colliderHeight;
                collider.direction = bulletConfigData.colliderDirection;
                collider.isTrigger = bulletConfigData.isColliderTrigger;

                Rigidbody rigidbody = view.GetOrCreateRigidbody();
                rigidbody.useGravity = false;
                rigidbody.drag = 0;
                rigidbody.angularDrag = 0;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rigidbody.freezeRotation = true;
                rigidbody.velocity = Vector3.zero;
            }else
            {

            }
            return bulletEntity;
        }
    }
}
