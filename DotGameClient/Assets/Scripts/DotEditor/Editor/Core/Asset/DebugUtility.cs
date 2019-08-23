#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace Leyoutech.Utility
{
	public static class DebugUtility
	{
		public const string DEFAULT_TAG = "Default";
		public const string LOG_SPLIT1 = "🐷";
		public const string LOG_SPLIT2 = ":";

		private static System.Diagnostics.Stopwatch ms_Stopwatch;

		static DebugUtility()
		{
			ms_Stopwatch = new System.Diagnostics.Stopwatch();
			ms_Stopwatch.Start();
		}

		public static long GetMillisecondsSinceStartup()
		{
			return ms_Stopwatch.ElapsedMilliseconds;
		}

		public static long GetTicksSinceStartup()
		{
			return ms_Stopwatch.ElapsedTicks;
		}

		/// <summary>
		/// Log<see cref="LogLightmapSetting"/>的信息
		/// </summary>
		public static void LogLightmapSetting()
		{
			Debug.Log("lightmaps length " + LightmapSettings.lightmaps.Length);
			for (int iLightmapData = 0; iLightmapData < LightmapSettings.lightmaps.Length; iLightmapData++)
			{
				LightmapData iterLightmapData = LightmapSettings.lightmaps[iLightmapData];
				Debug.Log("lightmapColor", iterLightmapData.lightmapColor);
				Debug.Log("lightmapDir", iterLightmapData.lightmapDir);
				Debug.Log("shadowMask", iterLightmapData.shadowMask);
			}

			Debug.Log("lightmapsMode " + LightmapSettings.lightmapsMode);
			Debug.Log("lightProbes length" + LightmapSettings.lightProbes.bakedProbes.Length);
		}

		public static float ConvertTicksToFPS(long ticks)
		{
			return 10000000.0f / ticks;
		}

		public static void Log(string tag, string message)
		{
			Debug.Log(FormatLog(tag, message));
		}

		public static void Log(string tag, string message, UnityEngine.Object context)
		{
			Debug.Log(FormatLog(tag, message), context);
		}

		public static void LogWarning(string tag, string message)
		{
			Debug.LogWarning(FormatLog(tag, message));
		}

		public static void LogWarning(string tag, string message, UnityEngine.Object context)
		{
			Debug.LogWarning(FormatLog(tag, message), context);
		}

		public static void LogError(string tag, string message)
		{
			Debug.LogError(FormatLog(tag, message));
		}

		public static void LogError(string tag, string message, UnityEngine.Object context)
		{
			Debug.LogError(FormatLog(tag, message), context);
		}
        public static void LogErrorFormat(string tag, string format, params object[] args)
		{
            string message = string.Format(format, args);
			Debug.LogError(FormatLog(tag, message));
		}
		public static bool Assert(bool condition, string message, bool displayDialog = true)
		{
			if (!condition)
			{
				Debug.Assert(condition, message);
#if UNITY_EDITOR
				if (displayDialog)
				{
					EditorUtility.DisplayDialog("Assert Failed", message, "OK");
				}
				Debug.Break();
#endif
			}
			return condition;
		}
		
		public static bool Assert(bool condition, string message, UnityEngine.Object context, bool displayDialog = true)
		{
			if (!condition)
			{
				Debug.Assert(condition, message, context);
#if UNITY_EDITOR
				if (displayDialog)
				{
					EditorUtility.DisplayDialog("Assert Failed", message, "OK");
				}
				Debug.Break();
#endif
			}
			return condition;
		}

		public static bool Assert(bool condition,string tag, string message, bool displayDialog = true)
		{
			return Assert(condition, FormatLog(tag, message), displayDialog);
		}

		public static bool Assert(bool condition, string tag, string message, UnityEngine.Object context, bool displayDialog = true)
		{
			return Assert(condition, FormatLog(tag, message), context, displayDialog);
		}

		public static string FormatLog(string tag, string message)
		{
			return string.Format("{0}{1}{2}{3}", LOG_SPLIT1, tag, LOG_SPLIT2, message);
		}

		public static void ParserLog(string log, out string tag, out string message)
		{
			if (log.StartsWith("🐷"))
			{
				int split2Index = log.IndexOf(LOG_SPLIT2);
				if (split2Index > 0)
				{
					tag = log.Substring(LOG_SPLIT1.Length, split2Index - LOG_SPLIT1.Length);
					message = log.Substring(split2Index + 1);
				}
				else
				{
					tag = DEFAULT_TAG;
					message = log;
				}
			}
			else
			{
				tag = DEFAULT_TAG;
				message = log;
			}
		}

		public static bool Assert<T>(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<T> asyncOperationHandle
			, string tag
			, string message
			, bool displayDialog = true)
		{
			if (asyncOperationHandle.OperationException == null)
			{
				if (asyncOperationHandle.IsDone)
				{
					return true;
				}
				else
				{
					return Assert(false, FormatLog(tag, message + ": async operation handle not done"), displayDialog);
				}
			}
			else
			{
				return Assert(false, FormatLog(tag, message + ": async operation handle throw Exception:\n" + asyncOperationHandle.OperationException.ToString()), displayDialog);
			}
		}
	}
}