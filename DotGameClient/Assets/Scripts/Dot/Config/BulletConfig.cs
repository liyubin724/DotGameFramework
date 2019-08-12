using Dot.Core.Entity.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dot.Config
{
    [Serializable]
   public class BulletConfigData
    {
        public int id;
        public float lifeTime = 0.0f;
        public string skeletonPath;
        public string timelinePath;
        public TargetType targetType = TargetType.None;

        public bool hasPhysics = true;
        public Vector3 colliderCenter = Vector3.zero;
        public float colliderRadius = 0.001f;
        public float colliderHeight = 0.1f;
        public int colliderDirection = 2;
        public bool isColliderTrigger = true;

    }

    [Serializable]
    public class BulletConfig
    {
        public List<BulletConfigData> configs = new List<BulletConfigData>();
    }
}
