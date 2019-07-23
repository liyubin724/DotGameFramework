using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Game.Physics
{
    public class RigibodySystem : AGameEntityReactiveSystem
    {
        public RigibodySystem(Contexts contexts, Services services) : base(contexts, services)
        {
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach(var entity in entities)
            {
                VirtualView view = entity.view.view as VirtualView;
                if(view!=null)
                {
                    Rigidbody rigidbody = view.GetOrCreateRigidbody();
                    rigidbody.useGravity = entity.rigidbody.useGravity;
                    rigidbody.drag = entity.rigidbody.drag;
                    rigidbody.angularDrag = entity.rigidbody.angularDrag;
                    rigidbody.collisionDetectionMode = entity.rigidbody.mode;
                    rigidbody.freezeRotation = entity.rigidbody.freezeRotation;
                    rigidbody.velocity = entity.rigidbody.velocity;
                }
            }
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView && entity.hasRigidbody;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Rigidbody);
        }
    }
}
