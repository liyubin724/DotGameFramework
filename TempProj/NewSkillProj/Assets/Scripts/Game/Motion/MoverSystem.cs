using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class MoverSystem : IExecuteSystem  
{
	private readonly Contexts contexts;
	private readonly Services services;
    private IGroup<GameEntity> movers = null;

	public MoverSystem(Contexts contexts,Services services)
	{
		this.contexts = contexts;
		this.services = services;

        movers = contexts.game.GetGroup(GameMatcher.Mover);
	}

	public void Execute() 
	{
		foreach(var e in movers.GetEntities())
        {
            if(e.isMover)
            {
                if(e.hasMotionType)
                {
                    if(e.motionType.value == MotionType.Linear)
                    {
                        float curSpeed = e.hasSpeed ? e.speed.value : 0.0f;
                        float curAcc = e.hasAcceleration ? e.acceleration.value : 0.0f;

                        float speed = curSpeed + curAcc * services.timeService.DeltaTime();
                        Vector3 deltaPosition = (e.direction.value * curSpeed + (e.direction.value * curAcc) * services.timeService.DeltaTime()) * services.timeService.DeltaTime();

                        e.ReplaceSpeed(speed);
                        e.ReplacePosition(e.position.value + deltaPosition);
                    }
                }
            }
        }
	}
}
