using UnityEngine;

namespace Dot.Core.Util
{
    public class DontDestoryBehaviour : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyHandler.AddTransform(transform);
            Destroy(this);
        }
    }
}
