
using System.Diagnostics;

namespace Dot.Core.Logger
{
    public class DebugLogger
    {
        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Log(string msg)=> UnityEngine.Debug.Log(msg);

        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void LogError(string msg) => UnityEngine.Debug.LogError(msg);

        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(string msg) => UnityEngine.Debug.LogWarning(msg);

        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void Log(bool condition, string msg) 
        {
            if (condition) Log(msg);
        }
        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void LogError(bool condition, string msg)
        {
            if (condition) LogError(msg);
        }
        
        [Conditional("DEBUG")]
        [Conditional("UNITY_EDITOR")]
        public static void LogWarning(bool condition, string msg)
        {
            if (condition) LogError(msg);
        }
    }
}
