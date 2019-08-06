using Dot.Core.Event;
using UnityEngine;

namespace Dot.Core.Entity
{
    public enum ColliderType
    {
        Capsule,
    }

    public enum MoveControlType
    {
        Normal,
        Rigidbody,
    }

    public class PhysicsVirtualView : VirtualView
    {
        private PhysicsBehaviour phyBehaviour = null;
        public PhysicsVirtualView(string name) : base(name)
        {
        }

        public PhysicsVirtualView(string name, Transform parent) : base(name, parent)
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

        public MoveControlType ControlType { get; set; } = MoveControlType.Normal;

        protected override void OnPosition(EventData eventData)
        {
            if(ControlType == MoveControlType.Normal)
            {
                base.OnPosition(eventData);
            }else if(ControlType == MoveControlType.Rigidbody)
            {
                GetOrCreateRigidbody().MovePosition(eventData.GetValue<Vector3>());
            }
        }

        protected override void OnDirection(EventData eventData)
        {
            if(ControlType == MoveControlType.Normal)
            {
                base.OnDirection(eventData);
            }else if(ControlType == MoveControlType.Rigidbody)
            {
                GetOrCreateRigidbody().MoveRotation(Quaternion.Euler(eventData.GetValue<Vector3>()));
            }
        }

        public override void DoReset()
        {
            if (rigidbody != null) Object.Destroy(rigidbody);
            rigidbody = null;
            if(collider!=null) Object.Destroy(collider);
            collider = null;
            Object.Destroy(phyBehaviour);
            phyBehaviour = null;

            ControlType = MoveControlType.Normal;

            base.DoReset();
        }
    }
}
