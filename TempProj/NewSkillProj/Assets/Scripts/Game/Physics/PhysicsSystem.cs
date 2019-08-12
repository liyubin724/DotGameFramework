using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitas;
using UnityEngine;

namespace Game.Physics
{
    public class PhysicsSystem : AGameEntitySystem,IExecuteSystem
    {
        private IGroup<GameEntity> bulletGroup = null;
        public PhysicsSystem(Contexts contexts, Services services) : base(contexts, services)
        {
            
        }

        public void Execute()
        {
            bulletGroup = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Bullet, GameMatcher.View, GameMatcher.Collider, GameMatcher.Rigidbody));
            foreach (var entity in bulletGroup.GetEntities())
            {
                if (entity.isMarkDestroy) continue;
                Vector3 prePosition = entity.hasPrePosition ? entity.prePosition.value : entity.position.value;
                Vector3 curPosition = entity.position.value;

                VirtualView view = entity.view.view as VirtualView;
                CapsuleCollider capCol = (CapsuleCollider)view.GetOrCreateCollider(entity.collider.colliderType);
                Vector3 dir = (curPosition - prePosition).normalized;
                float distance = (curPosition - prePosition).sqrMagnitude;
                Vector3 colDir = Vector3.zero;
                if (capCol.direction == 0)
                {
                    colDir = view.RootTransform.right;
                }
                else if (capCol.direction == 1)
                {
                    colDir = view.RootTransform.up;
                }
                else if (capCol.direction == 2)
                {
                    colDir = view.RootTransform.forward;
                }
                Vector3 offset = colDir * capCol.height * 0.5f;

                Vector3 centerPos = prePosition + capCol.center;
                Vector3 position1 = centerPos + offset;
                Vector3 position2 = centerPos - offset;

                if(UnityEngine.Physics.CapsuleCast(position1, position2, capCol.radius, dir, out RaycastHit hit, distance,1<<LayerMask.NameToLayer("SpacecraftOtherPlayer")))
                {
                    entity.ReplaceRaycatHit(hit);
                }
            }
        }

    }
}
