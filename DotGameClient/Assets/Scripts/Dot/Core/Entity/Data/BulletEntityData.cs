using Dot.Core.Event;

namespace Dot.Core.Entity.Data
{
    public class BulletEntityData : EntityBaseData,IMoveData,ITargetData
    {
        private EntityMoveData moveData = null;
        public EntityMoveData GetMoveData() => moveData;

        private EntityTargetData targetData = null;
        public EntityTargetData GetTargetData() => targetData;

        public BulletEntityData(EventDispatcher dispatcher) : base(dispatcher)
        {
            moveData = new EntityMoveData();
            targetData = new EntityTargetData();
        }


    }
}
