using Entitas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ColliderType
{
    Capsule,
}

[Game]
public class ColliderComponent : IComponent
{
    public ColliderType colliderType = ColliderType.Capsule;
}