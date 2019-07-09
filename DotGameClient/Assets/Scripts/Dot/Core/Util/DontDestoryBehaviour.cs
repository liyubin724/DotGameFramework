using UnityEngine;

namespace Dot.Core.Util
{
    public class DontDestoryBehaviour : MonoBehaviour
    {
        void Awake()
        {
            DontDestoryHandler.AddTransform(transform);
            Destroy(this);
        }
    }
}
