using System;
using System.Text;
using UnityEngine;

namespace Leyoutech.Utility
{
	public static class StringUtility
	{
		/// <summary>
		/// Cache下来避免每次使用时new一个
		/// 只能在主线程中使用
		/// 不能在协程中跨过yield return去使用，For Error Example：
		/// <code>
		/// StringBuilder stringBuilder = AllocStringBuilderCache();
		/// yield return null;
		/// stringBuilder.Append("");
		/// </code>
		/// 
		/// 使用方法：
		///		<see cref="AllocStringBuilderCache"/>
		///		<see cref="ReleaseStringBuilderCache"/>
		///		<see cref="ReleaseStringBuilderCacheAndReturnString"/>
		/// </summary>
		private static StringBuilder ms_StringBuilderCache;
		private static System.Security.Cryptography.MD5 ms_DefaultMd5;

		static StringUtility()
		{
			ms_StringBuilderCache = new StringBuilder();
		}

		/// <summary>
		/// 例：
		///		E:\qwe\sdf\wts.gwe 返回wts
		/// </summary>
		/// <param name="path">完整路径</param>
		/// <returns>文件名(不包括扩展名)</returns>
		public static string SubFileNameFromPath(string path)
		{
			int index = path.LastIndexOfAny(new char[] { '\\', '/' });
			if (index > 0)
			{
				path = path.Substring(index + 1);
			}
			index = path.LastIndexOf('.');
			if (index > 0)
			{
				path = path.Substring(0, index);
			}
			return path;
		}

		/// <summary>
		/// <see cref="ms_StringBuilderCache"/>返回一个空的StringBuilder
		/// </summary>
		public static StringBuilder AllocStringBuilderCache()
		{
#if UNITY_EDITOR
			DebugUtility.Assert(ms_StringBuilderCache.Length == 0
				, "AllocStringBuilderCache时ms_StringBuilderCache不为空，是不是上次AllocStringBuilderCache后没调用ReleaseStringBuilderCache");
			ms_StringBuilderCache.Clear();
#endif
			return ms_StringBuilderCache;
		}

		/// <summary>
		/// 和<see cref="AllocStringBuilderCache"/>的区别是这个方法返回的StringBuilder不一定是空的
		/// </summary>
		public static StringBuilder GetStringBuilderCache()
		{
			return ms_StringBuilderCache;
		}

		/// <summary>
		/// <see cref="ms_StringBuilderCache"/>
		/// </summary>
		public static void ReleaseStringBuilderCache()
		{
			ms_StringBuilderCache.Clear();
		}

		/// <summary>
		/// <see cref="ms_StringBuilderCache"/>
		/// </summary>
		public static string ReleaseStringBuilderCacheAndReturnString()
		{
			string str = ms_StringBuilderCache.ToString();
			ms_StringBuilderCache.Clear();
			return str;
		}

		/// <summary>
		/// 计算一个字符串的MD5
		/// </summary>
		public static string CalculateMD5Hash(string input)
		{
			if (ms_DefaultMd5 == null)
			{
				ms_DefaultMd5 = System.Security.Cryptography.MD5.Create();
			}

			byte[] inputBytes = Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = ms_DefaultMd5.ComputeHash(inputBytes);

			StringBuilder stringBuilder = AllocStringBuilderCache();
			for (int iByte = 0; iByte < hashBytes.Length; iByte++)
			{
				stringBuilder.Append(hashBytes[iByte].ToString("X2"));
			}
			return ReleaseStringBuilderCacheAndReturnString();
		}

		/// <summary>
		/// 把一个字符串转换为变量名
		/// </summary>
		public static string ConvertToVariableName(string value, char replace = '_')
		{
			string variableName = string.Empty;
			for (int iChar = 0; iChar < value.Length; iChar++)
			{
				char iterChar = value[iChar];
				if (iterChar == '_'
					|| char.IsLetterOrDigit(iterChar))
				{
					variableName += iterChar;
				}
				else
				{
					variableName += replace;
				}
			}
			if (char.IsNumber(variableName[0]))
			{
				variableName = replace + variableName;
			}
			return variableName;
		}
	}
}