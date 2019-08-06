﻿using Dot.Core.Entity.Data;
using UnityEngine;

namespace Dot.Core.Entity.Controller
{
    public class EntityMoveController : AEntityController
    {
        public override void DoUpdate(float deltaTime)
        {
            if(!Enable)
            {
                return;
            }

            if(entity.EntityData==null || entity.EntityData.MoveData ==null || !entity.EntityData.MoveData.GetIsMover())
            {
                return;
            }

            MoveData moveData = entity.EntityData.MoveData;
            MotionCurveType motionType = moveData.GetMotionType();
            if (motionType == MotionCurveType.None)
                return;

            TargetData targetData = entity.EntityData.TargetData;
            if(targetData!=null)
            {
                Vector3 targetPosition = targetData.GetPosition();
                Vector3 direction = (targetPosition - entity.EntityData.GetPosition()).normalized;
                if((targetPosition-entity.EntityData.GetPosition()).magnitude <= 0.001f ||
                    Vector3.Dot(entity.EntityData.GetDirection(), direction) < 0)
                {
                    moveData.SetIsMover(false);

                    entity.SendEvent(EntityInnerEventConst.ARRIVED_TARGET_ID);
                    return;
                }

                entity.EntityData.SetDirection(direction);
            }

            if(motionType == MotionCurveType.Linear)
            {
                MoveLinear(deltaTime);
            }
        }
        

        private void MoveLinear(float deltaTime)
        {
            MoveData moveData = entity.EntityData.MoveData;
            float acceleration = moveData.GetAcceleration();
            Vector3 direction = entity.EntityData.GetDirection();
            float maxSpeed = moveData.GetMaxSpeed();

            float targetSpeed = moveData.GetSpeed() + acceleration * deltaTime;
            if(maxSpeed != 0f && targetSpeed> maxSpeed)
            {
                targetSpeed = maxSpeed;
            }
            moveData.SetMaxSpeed(targetSpeed);

            Vector3 deltaPostion = direction * targetSpeed * deltaTime + direction * acceleration * deltaTime * deltaTime;

            entity.EntityData.SetPosition(entity.EntityData.GetPosition() + deltaPostion);
        }
    }
}
