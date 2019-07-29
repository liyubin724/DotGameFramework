using UnityEngine;

namespace Dot.Core.Avatar
{
    public class AvatarRendererPart : ScriptableObject
    {
        public string rendererNodeName = "";
        public string rootBoneName = "";
        public Mesh mesh = null;
        public Material[] materials = new Material[0];
        public string[] boneNames = new string[0];
    }
}
