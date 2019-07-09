using UnityObject = UnityEngine.Object;

namespace Dot.Core.Util
{
    public static class UnityObjectExtension
    {
        public static bool IsNull(this UnityObject obj)
        {
            return obj == null;
        }
    }
}
