using UnityEngine;

public class MotionService : Service
{
    public MotionService(Contexts contexts) : base(contexts)
    {
    }

    public override void DoDestroy()
    {
        
    }

    public override void DoReset()
    {
        
    }

    public Vector3 GetSpeed(Vector3 direction,float speed,float acceleration,float deltaTime)
    {
        return direction * speed + (direction * acceleration) * deltaTime;
    }

    public Vector3 GetDeltaPosition(Vector3 direction, float speed, float acceleration, float deltaTime)
    {
        return direction*speed * deltaTime + (direction * acceleration) * deltaTime * deltaTime * 0.5f;
    }
}