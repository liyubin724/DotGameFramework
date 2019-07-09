using System;
using UnityEngine;

namespace Dot.Core.Avator
{
    public enum AvatarPartType
    {
        None = 0,
        Jiao,
        Shen,
        Shou,
        Tou,
        Max,
    }

    public class AvatarRendererPart : ScriptableObject
    {
        public Mesh mesh = null;
        public Material[] materials = new Material[0];
        public string[] boneNames = new string[0];
        public string rootBoneName = "";
        public string rendererNodeName = "";
    }

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

    public class AvatarPartInstance
    {
        public AvatarPartType partType = AvatarPartType.None;
        public Renderer[] renderers = new Renderer[0];
        public GameObject[] gameObjects = new GameObject[0];
    }
}
