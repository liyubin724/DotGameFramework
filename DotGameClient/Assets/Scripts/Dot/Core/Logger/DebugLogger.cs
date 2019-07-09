#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
using System;
#endif

namespace Dot.Core.Logger
{
    public class DebugLogger
    {
        public static void Log(string msg)
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Debug.Log(msg);
#else
            ConsoleColor cc = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(msg);

            Console.ForegroundColor = cc;
#endif
        }

        public static void LogError(string msg)
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Debug.LogError(msg);
#else
            ConsoleColor cc = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);

            Console.ForegroundColor = cc;
#endif
        }

        public static void LogWarning(string msg)
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Debug.LogWarning(msg);
#else
            ConsoleColor cc = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);

            Console.ForegroundColor = cc;
#endif
        }
    }
}
