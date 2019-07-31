using Dot.Core.Entity;
using Dot.Core.Entity.Controller;

namespace Game.Battle.Entity
{
    public class BulletEntityBuilder : AEntityBuilder
    {
        public override EntityObject CreateEntityObject(long uniqueID, int category)
        {
            EntityObject bulletEntity = CreateEntity(uniqueID, category);

            EntityViewController viewController = AddControllerToEntity<EntityViewController>(bulletEntity,EntityControllerConst.VIEW_INDEX);
            VirtualView view = new VirtualView(bulletEntity.Name, Context.EntityRootTransfrom);
            viewController.SetView(view);

            AddControllerToEntity<EntitySkeletonController>(bulletEntity, EntityControllerConst.SKELETON_INDEX);
            AddControllerToEntity<EntityMoveController>(bulletEntity, EntityControllerConst.MOVE_INDEX);
            AddControllerToEntity<BulletPhysicsController>(bulletEntity, EntityControllerConst.PHYSICS_INDEX);

            return bulletEntity;
        }
    }
}
