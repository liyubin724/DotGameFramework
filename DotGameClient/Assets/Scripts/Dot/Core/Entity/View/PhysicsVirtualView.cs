using Dot.Core.Event;
using UnityEngine;

namespace Dot.Core.Entity
{
    public enum ColliderType
    {
        Capsule,
    }

    public class PhysicsVirtualView : VirtualView
    {
        private PhysicsBehaviour phyBehaviour = null;
        public PhysicsVirtualView(string name, EventDispatcher dispatcher) : base(name, dispatcher)
        {
        }

        public PhysicsVirtualView(string name, Transform parent, EventDispatcher dispatcher) : base(name, parent, dispatcher)
        {
            phyBehaviour = RootGameObject.GetComponent<PhysicsBehaviour>();
            if(phyBehaviour == null)
            {
                phyBehaviour = RootGameObject.AddComponent<PhysicsBehaviour>();
            }
            phyBehaviour.Entity = entity;
        }

        private Rigidbody rigidbody = null;
        public Rigidbody GetOrCreateRigidbody()
        {
            if (rigidbody == null)
            {
                rigidbody = RootGameObject.GetComponent<Rigidbody>();
            }
            if (rigidbody == null)
            {
                rigidbody = RootGameObject.AddComponent<Rigidbody>();
            }
            return rigidbody;
        }

        private Collider collider = null;
        public Collider GetOrCreateCollider(ColliderType colliderType)
        {
            if (collider == null)
            {
                collider = RootGameObject.GetComponent<Collider>();
            }
            if (colliderType == ColliderType.Capsule)
            {
                if (collider != null)
                {
                    if (collider.GetType() != typeof(CapsuleCollider))
                    {
                        Debug.LogError("Collider not Same");
                        return null;
                    }
                }
                else
                {
                    collider = RootGameObject.AddComponent<CapsuleCollider>();
                }
            }

            return collider;
        }
    }
}
