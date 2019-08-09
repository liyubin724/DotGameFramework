using Dot.Core.Entity;
using Dot.Core.Entity.Controller;
using UnityEngine;

namespace Game.Entity
{
    public class BulletEntityBuilder : AEntityBuilder
    {
        protected override void OnCreate(EntityObject entity)
        {
            EntityViewController viewController = entity.GetController<EntityViewController>(EntityControllerConst.VIEW_INDEX);
            PhysicsVirtualView view = new PhysicsVirtualView(entity.Name, Context.EntityRootTransfrom);
            view.ControlType = MoveControlType.Normal;
            viewController.SetView(view);

            
        }

        protected override void OnDelete(EntityObject entity)
        {
        }
    }
}
