using UnityEngine;

namespace Dot.Core.Avatar
{
    public class AvatarRendererPart : ScriptableObject
    {
        public Mesh mesh = null;
        public Material[] materials = new Material[0];
        public string[] boneNames = new string[0];
        public string rootBoneName = "";
        public string rendererNodeName = "";
    }
}
