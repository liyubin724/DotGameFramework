using Dot.Core.Entity;
using Dot.Core.Entity.Controller;

namespace Game.Battle.Entity
{
    public class BulletEntityBuilder : AEntityBuilder
    {
        public override EntityObject CreateEntityObject(long uniqueID, int entityType)
        {
            EntityObject bulletEntity = new EntityObject();
            bulletEntity.Category = entityType;
            bulletEntity.UniqueID = uniqueID;
            bulletEntity.Name = $"Bullet_{entityType}_{uniqueID}";

            EntityViewController viewController = AddControllerToEntity<EntityViewController>(bulletEntity,EntityControllerConst.VIEW_INDEX);
            VirtualView view = new VirtualView(bulletEntity.Name, Context.EntityRootTransfrom);
            viewController.SetView(view);

            AddControllerToEntity<EntitySkeletonController>(bulletEntity, EntityControllerConst.SKELETON_INDEX);
            AddControllerToEntity<EntityMoveController>(bulletEntity, EntityControllerConst.MOVE_INDEX);
            AddControllerToEntity<BulletPhysicsController>(bulletEntity, EntityControllerConst.PHYSICS_INDEX);

            return bulletEntity;
        }

        public override void DestroyEntityObject(EntityObject entity)
        {
            if (entity == null) return;

            int[] indexes = null;
            AEntityController[] controllers = null;
            entity.RemoveAllController(out indexes, out controllers);

            if(indexes!=null && indexes.Length>0)
            {
                for(int i =0;i<indexes.Length;++i)
                {
                    ReleaseController(indexes[i], controllers[i]);
                }
            }
        }
    }
}
