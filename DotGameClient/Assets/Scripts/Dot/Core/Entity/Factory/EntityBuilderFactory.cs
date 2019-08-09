using Dot.Core.Entity;

namespace Game.Entity
{
    public static class EntityBuilderFactory
    {
        public static void RegisterEntityBuilder(EntityContext context)
        {
            context.RegisterEntityBuilder(EntityCategroyConst.SHIP, new ShipEntityBuilder());
            context.RegisterEntityBuilder(EntityCategroyConst.BULLET, new BulletEntityBuilder());
        }
    }
}
