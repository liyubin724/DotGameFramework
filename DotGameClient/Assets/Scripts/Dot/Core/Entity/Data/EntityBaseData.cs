using Dot.Core.Event;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Entity.Data
{
    public class EntityBaseData
    {
        public int ConfigID { get; set; } = 0;
        public long OwnerUniqueID { get; set; } = 0;

        private EventDispatcher dispatcher;
        public EntityBaseData(EventDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void SendEvent(int eventID, SystemObject[] datas = null) => dispatcher?.TriggerEvent(eventID, 0, datas);

        private Vector3 position = Vector3.zero;
        public Vector3 GetPosition() => position;
        public void SetPosition(Vector3 position)
        {
            this.position = position;
            SendEvent(EntityInnerEventConst.POSITION_ID);
        }

        private Vector3 direction = Vector3.zero;
        public Vector3 GetDirection() => direction;
        public void SetDirection(Vector3 direction)
        {
            this.direction = direction;
            SendEvent(EntityInnerEventConst.POSITION_ID);
        }
    }
}
