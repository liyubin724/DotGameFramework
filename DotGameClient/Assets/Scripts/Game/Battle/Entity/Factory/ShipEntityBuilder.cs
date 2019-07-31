using Dot.Core.Entity;
using Dot.Core.Entity.Controller;

namespace Game.Battle.Entity
{
    public class ShipEntityBuilder : AEntityBuilder
    {
        public override EntityObject CreateEntityObject(long uniqueID, int category)
        {
            EntityObject shipEntity = CreateEntity(uniqueID, category);

            EntityViewController viewController = AddControllerToEntity<EntityViewController>(shipEntity, EntityControllerConst.VIEW_INDEX);
            VirtualView view = new VirtualView(shipEntity.Name, Context.EntityRootTransfrom);
            viewController.SetView(view);

            AddControllerToEntity<EntitySkeletonController>(shipEntity, EntityControllerConst.SKELETON_INDEX);
            AddControllerToEntity<EntityMoveController>(shipEntity, EntityControllerConst.MOVE_INDEX);
            AddControllerToEntity<BulletPhysicsController>(shipEntity, EntityControllerConst.PHYSICS_INDEX);

            return null;
        }
    }
}
