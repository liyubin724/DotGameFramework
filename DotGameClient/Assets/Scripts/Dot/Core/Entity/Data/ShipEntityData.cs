using Dot.Core.Event;

namespace Dot.Core.Entity.Data
{
    public class ShipEntityData : EntityBaseData
    {
        private EntityMoveData moveData = null;
        public EntityMoveData GetMoveData() => moveData;

        public ShipEntityData(EventDispatcher dispatcher) : base(dispatcher)
        {
        }
    }
}
