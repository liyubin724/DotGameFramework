using Dot.Core.Event;
using UnityEngine;
using SystemObject = System.Object;

namespace Dot.Core.Entity.Data
{
    public class EntityData
    {
        public int ConfigID { get; set; } = 0;
        public long OwnerUniqueID { get; set; } = 0;

        private EntityEventData eventData = new EntityEventData();
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

        private EntityMoveData moveData = null;
        public EntityMoveData MoveData
        {
            get
            {
                return moveData;
            }
            set
            {
                moveData = value;
                if(moveData == null)
                {
                    moveData.eventData = eventData;
                }
            }
        }
        
        public EntityTargetData TargetData { get; set; }

        private EntityTimeLineData timeLineData = null;
        public EntityTimeLineData TimeLineData
        {
            get
            {
                return timeLineData;
            }
            set
            {
                timeLineData = value;
                if(timeLineData!=null)
                {
                    timeLineData.eventData = eventData;
                }
            }
        }







    }
}
