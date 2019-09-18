using UnityObject = UnityEngine.Object;
using SystemObject = System.Object;

namespace Dot.Core.Util
{
    public static class UnityObjectExtension
    {
        public static bool IsNull(this UnityObject obj)
        {
            return obj == null;
        }

        public static bool IsNull(SystemObject sysObj)
        {
            if (sysObj == null || sysObj.Equals(null))
            {
                return true;
            }

            return false;
        }
    }
}
