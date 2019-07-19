using Entitas;
using UnityEngine;

[Game]
public class CapsuleColliderComponent : IComponent
{
    public Vector3 center = Vector3.zero;
    public float radius = 0.0f;
    public float height = 0.0f;
    public int direction = 0;
    public bool isTrigger = false;
}