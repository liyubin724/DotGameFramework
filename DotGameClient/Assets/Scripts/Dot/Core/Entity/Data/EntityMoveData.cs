using UnityEngine;

namespace Dot.Core.Entity.Data
{
    public enum MotionCurveType
    {
        None,
        Linear,

    }

    public class EntityMoveData
    {
        internal EntityEventData eventData = null;

        private bool isMover = false;
        public bool GetIsMover() => this.isMover;
        public void SetIsMover(bool isMover) => this.isMover = isMover;

        private MotionCurveType motionType = MotionCurveType.None;
        public void SetMotionType(MotionCurveType mt)=> motionType = mt;
        public MotionCurveType GetMotionType() => motionType;

        private float speed = 0.0f;
        public float GetSpeed() => speed;
        public void SetSpeed(float speed) => this.speed = speed;

        private float acceleration = 0f;
        public float GetAcceleration() => acceleration;
        public float SetAcceleration(float acc) => acceleration = acc;

        private float maxSpeed = 0.0f;
        public float GetMaxSpeed() => maxSpeed;
        public void SetMaxSpeed(float maxSpeed) => this.maxSpeed = maxSpeed;
    }
}
