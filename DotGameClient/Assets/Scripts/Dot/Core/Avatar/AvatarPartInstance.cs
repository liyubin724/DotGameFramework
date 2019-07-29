using UnityEngine;

namespace Dot.Core.Avatar
{
    public class AvatarPartInstance
    {
        public AvatarPartType partType = AvatarPartType.None;
        public Renderer[] renderers = new Renderer[0];
        public GameObject[] gameObjects = new GameObject[0];
    }
}
