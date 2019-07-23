using Entitas;
using UnityEngine;

[Game]
public class RigidbodyComponent : IComponent
{
    public bool useGravity = false;
    public float drag = 0f;
    public float angularDrag = 0f;
    public bool isKinematic = false;
    public CollisionDetectionMode mode = CollisionDetectionMode.ContinuousDynamic;
    public RigidbodyConstraints constraints = RigidbodyConstraints.None;
    public Vector3 velocity = Vector3.zero;
}
