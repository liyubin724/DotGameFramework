using Dot.Config;
using Dot.Core.Entity.Controller;
using Dot.Core.Entity.Data;
using UnityEngine;

namespace Dot.Core.Entity
{
    public static class EntityFactroy
    {
        public static EntityObject CreateBullet(int configID,Vector3 position,Vector3 direction)
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

            bulletEntity.EntityData.SetPosition(position);
            bulletEntity.EntityData.SetDirection(direction);

            BulletConfigData bulletConfigData = ConfigManager.GetInstance().GetBulletConfig(configID);
            bulletEntity.EntityData.ConfigID = configID;

            bulletEntity.EntityData.TimeLineData = new EntityTimeLineData();
            if(!string.IsNullOrEmpty(bulletConfigData.timelineAddress))
            {
                bulletEntity.EntityData.TimeLineData.SetTrackControl(ConfigManager.GetInstance().GetTimeLineConfig(bulletConfigData.timelineAddress));
            }
            
            EntityViewController viewController = bulletEntity.GetController<EntityViewController>(EntityControllerConst.VIEW_INDEX);
            PhysicsVirtualView view = viewController.GetView<PhysicsVirtualView>();

            CapsuleCollider collider = view.GetOrCreateCollider(ColliderType.Capsule) as CapsuleCollider;
            collider.center = Vector3.zero;
            collider.radius = 0.001f;
            collider.height = 0.1f;
            collider.direction = 2;
            collider.isTrigger = true;

            Rigidbody rigidbody = view.GetOrCreateRigidbody();
            rigidbody.useGravity = false;
            rigidbody.drag = 0;
            rigidbody.angularDrag = 0;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidbody.freezeRotation = true;
            rigidbody.velocity = Vector3.zero;

            return bulletEntity;
        }
    }
}
