using UnityEngine;

namespace Dot.Core.Generic
{
    public static class CatmullRom
    {
        public static Vector3 GetPosition(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float rate)
        {
            return point1 * (-0.5f * rate * rate * rate + rate * rate - 0.5f * rate) +
                point2 * (1.5f * rate * rate * rate - 2.5f * rate * rate + 1.0f) +
                point3 * (-1.5f * rate * rate * rate + 2.0f * rate * rate + 0.5f * rate) +
                point4 * (0.5f * rate * rate * rate - 0.5f * rate * rate);
        }
    }
}
