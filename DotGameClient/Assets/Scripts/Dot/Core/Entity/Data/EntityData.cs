using Dot.Core.Event;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Entity.Data
{
    public class EntityData
    {
        private EventData eventData = new EventData();
        public void SetEventDispatcher(EventDispatcher dispatcher)
        {
            eventData.Dispatcher = dispatcher;
        }

        private Vector3 position = Vector3.zero;
        public Vector3 GetPosition() => position;
        public void SetPosition(Vector3 position)
        {
            this.position = position;
            eventData.SendEvent(EntityInnerEventConst.POSITION_ID);
        }

        private Vector3 direction = Vector3.zero;
        public Vector3 GetDirection() => direction;
        public void SetDirection(Vector3 direction)
        {
            this.direction = direction;
            eventData.SendEvent(EntityInnerEventConst.POSITION_ID);
        }

        private MoveData moveData = null;
        public MoveData MoveData
        {
            get
            {
                return moveData;
            }
            set
            {
                moveData = value;
                if(moveData!=null)
                {
                    moveData.eventData = eventData;
                }
            }
        }


        public TargetData TargetData { get; set; }










    }
}
