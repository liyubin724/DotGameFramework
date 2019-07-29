using System;
using UnityEngine;

namespace Dot.Core.Avatar
{
    [Serializable]
    public class AvatarPrefabPart
    {
        public string bindNodeName = "";
        public GameObject prefabGO = null;
    }

    public class AvatarPart : ScriptableObject
    {
        public AvatarPartType partType = AvatarPartType.None;
        public AvatarRendererPart[] rendererParts = new AvatarRendererPart[0];
        public AvatarPrefabPart[] prefabParts = new AvatarPrefabPart[0];
    }
}
