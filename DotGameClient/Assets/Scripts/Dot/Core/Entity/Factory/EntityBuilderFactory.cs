using Dot.Core.Entity;

namespace Game.Entity
{
    public static class EntityBuilderFactory
    {
        public static void RegisterEntityBuilder(EntityContext context)
        {
            context.RegisterEntityBuilder(EntityCategroyConst.Ship, new ShipEntityBuilder());
            context.RegisterEntityBuilder(EntityCategroyConst.BULLET, new BulletEntityBuilder());
        }
    }
}
