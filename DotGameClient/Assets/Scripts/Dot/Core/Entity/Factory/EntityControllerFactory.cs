using Dot.Core.Entity;
using Dot.Core.Entity.Controller;

namespace Game.Entity
{
    public static class EntityControllerFactory
    {
        public static AEntityController GetController(int category,int controllerIndex)
        {
            if(controllerIndex == EntityControllerConst.SKELETON_INDEX)
            {
                return new EntitySkeletonController();
            }
            if(controllerIndex == EntityControllerConst.AVATAR_INDEX)
            {
                return new EntityAvatarController();
            }
            if (controllerIndex == EntityControllerConst.EFFECT_INDEX)
            {
                return new EntityEffectController();
            }
            if (controllerIndex == EntityControllerConst.MOVE_INDEX)
            {
                return new EntityMoveController();
            }
            if (controllerIndex == EntityControllerConst.PHYSICS_INDEX)
            {
                return new EntityPhysicsController();
            }
            if (controllerIndex == EntityControllerConst.VIEW_INDEX)
            {
                return new EntityViewController();
            }
            if(controllerIndex == EntityControllerConst.TIMELINE_INDEX)
            {
                if (category == EntityCategroyConst.BULLET)
                {
                    return new BulletTimeLineController();
                }
            }
            
            return null;
        }
    }
}
