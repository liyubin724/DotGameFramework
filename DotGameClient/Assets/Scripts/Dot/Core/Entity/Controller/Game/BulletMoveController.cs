using Dot.Core.Entity.Data;
using UnityEngine;

namespace Dot.Core.Entity.Controller
{
    public class BulletMoveController : EntityMoveController
    {
        public override void DoUpdate(float deltaTime)
        {
            if (!Enable || entity.EntityData == null)
            {
                return;
            }

            BulletEntityData bulletEntityData = entity.EntityData as BulletEntityData;
            EntityMoveData moveData = bulletEntityData.GetMoveData();

            if (moveData == null || !moveData.GetIsMover())
            {
                return;
            }
            MotionCurveType motionType = moveData.GetMotionType();
            if (motionType == MotionCurveType.None)
                return;

            EntityTargetData targetData = bulletEntityData.GetTargetData();
            if (targetData != null)
            {
                Vector3 targetPosition = targetData.GetPosition();
                Vector3 direction = (targetPosition - entity.EntityData.GetPosition()).normalized;
                if ((targetPosition - entity.EntityData.GetPosition()).magnitude <= 0.001f ||
                    Vector3.Dot(entity.EntityData.GetDirection(), direction) < 0)
                {
                    moveData.SetIsMover(false);
                    moveData.SetAccelerationSpeed(0f);

                    entity.SendEvent(EntityInnerEventConst.ARRIVED_TARGET_ID);
                    return;
                }

                entity.EntityData.SetDirection(direction);
            }

            if (motionType == MotionCurveType.Linear)
            {
                MoveLinear(moveData,deltaTime);
            }
        }
        
        private void MoveLinear(EntityMoveData moveData,float deltaTime)
        {
            float acceleration = moveData.GetAcceleration();
            Vector3 direction = entity.EntityData.GetDirection();
            float maxSpeed = moveData.GetMaxSpeed();

            moveData.SetAccelerationSpeed(moveData.GetAccelerationSpeed() + acceleration * deltaTime);
            float targetSpeed = moveData.GetSpeed() + moveData.GetAccelerationSpeed();
            if (maxSpeed != 0f && targetSpeed > maxSpeed)
            {
                targetSpeed = maxSpeed;
            }

            Vector3 deltaPostion = direction * targetSpeed * deltaTime + direction * acceleration * deltaTime * deltaTime;

            entity.EntityData.SetPosition(entity.EntityData.GetPosition() + deltaPostion);
        }
    }
}
