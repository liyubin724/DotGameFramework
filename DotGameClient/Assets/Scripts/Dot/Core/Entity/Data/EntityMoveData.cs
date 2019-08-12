using UnityEngine;

namespace Dot.Core.Entity.Data
{
    public enum MotionCurveType
    {
        None,
        Linear,

    }

    public interface IMoveData
    {
        EntityMoveData GetMoveData();
    }

    public class EntityMoveData
    {
        private bool isMover = false;
        public bool GetIsMover() => this.isMover;
        public void SetIsMover(bool isMover) => this.isMover = isMover;

        private MotionCurveType motionType = MotionCurveType.None;
        public void SetMotionType(MotionCurveType mt)=> motionType = mt;
        public MotionCurveType GetMotionType() => motionType;

        #region Speed
        private float originSpeed = 0.0f;
        public float GetOriginSpeed() => originSpeed;
        public void SetOriginSpeed(float speed) => this.originSpeed = speed;

        private float timeLineConstantSpeed = 0.0f;
        public float GetTimeLineConstantSpeed() => timeLineConstantSpeed;
        public void SetTimeLineConstantSpeed(float speed) => timeLineConstantSpeed = speed;
        private float timeLineRateSpeed = 0.0f;
        public float GetTimeLineRateSpeed() => timeLineRateSpeed;
        public void SetTimeLineRateSpeed(float rate) => timeLineRateSpeed = rate;
        private float timeLineOffsetConstantSpeed = 0.0f;
        public float GetTimeLineOffsetConstantSpeed() => timeLineOffsetConstantSpeed;
        public void SetTimeLineOffsetConstantSpeed(float offset) => timeLineOffsetConstantSpeed = offset;

        private float accelerationSpeed = 0.0f;
        public float GetAccelerationSpeed() => accelerationSpeed;
        public void SetAccelerationSpeed(float speed) => accelerationSpeed = speed;
        
        public float GetSpeed()
        {
            float speed = originSpeed;
            if(timeLineConstantSpeed!=0)
            {
                speed = timeLineConstantSpeed;
            }
            if(timeLineRateSpeed!=0)
            {
                speed *= timeLineRateSpeed;
            }
            speed += timeLineOffsetConstantSpeed;

            return speed;
        }

        #endregion

        #region Acceleration
        private float originAcceleration = 0f;
        public float GetOriginAcceleration() => originAcceleration;
        public float SetOriginAcceleration(float acc) => originAcceleration = acc;

        private float timeLineAcceleration = 0f;
        public float GetTimeLineAcceleration() => timeLineAcceleration;
        public void SetTimeLineAcceleration(float acc) => timeLineAcceleration = acc;

        public float GetAcceleration()
        {
            float acc = originAcceleration;
            if(timeLineAcceleration!=0)
            {
                acc = timeLineAcceleration;
            }

            return acc;
        }
        #endregion

        #region Max Speed
        private float originMaxSpeed = 0.0f;
        public float GetOriginMaxSpeed() => originMaxSpeed;
        public void SetOriginMaxSpeed(float maxSpeed) => this.originMaxSpeed = maxSpeed;

        private float timeLineMaxSpeed = 0.0f;
        public float GetTimeLineMaxSpeed() => timeLineMaxSpeed;
        public void SetTimeLineMaxSpeed(float speed) => timeLineMaxSpeed = speed;

        public float GetMaxSpeed()
        {
            float maxSpeed = originMaxSpeed==0f?float.MaxValue:originMaxSpeed;
            if(timeLineMaxSpeed!=0)
            {
                maxSpeed = Mathf.Min(maxSpeed, timeLineMaxSpeed);
            }
            return maxSpeed;
        }
        #endregion
    }
}
