﻿using UnityEngine;

namespace Dot.Core.Entity.Controller
{
    public class EntityMoveController : AEntityController
    {
        public EntityMoveController(EntityObject entity) : base(entity)
        {
        }

        protected override void AddEventListeners()
        {
            
        }

        protected override void RemoveEventListeners()
        {
            
        }

        public override void DoUpdate(float deltaTime)
        {
            if(!Enable)
            {
                return;
            }
        }

        public void MoveTo(Vector3 position)
        {
            Dispatcher.TriggerEvent(EntityEventConst.POSITION_ID, 0, position);
        }

        public void ForwardTo(Vector3 direction)
        {
            Dispatcher.TriggerEvent(EntityEventConst.DIRECTION_ID, 0, direction);
        }
    }
}
