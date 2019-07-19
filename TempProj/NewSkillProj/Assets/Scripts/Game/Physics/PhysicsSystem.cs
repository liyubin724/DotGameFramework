using Entitas;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : AGameEntityReactiveSystem
{
    public PhysicsSystem(Contexts contexts, Services services) : base(contexts, services)
    {
    }

    protected override void Execute(List<GameEntity> entities)
    {
        foreach(var e in entities)
        {
        }
    }

    protected override bool Filter(GameEntity entity)
    {
        return false;
    }

    protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
    {
        return context.CreateCollector(GameMatcher.AllOf(GameMatcher.VirtualView,GameMatcher.Collider, GameMatcher.Rigidbody, GameMatcher.AnyOf(GameMatcher.CapsuleCollider)));
    }
}