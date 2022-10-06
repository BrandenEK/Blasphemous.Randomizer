using System;
using Framework.Managers;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.FrameworkCore
{
	public class Log : GameSystem
	{
		public static void Raw(string text)
		{
			UnityEngine.Debug.Log(text);
		}

		public static void Trace(string module, string text, Object context = null)
		{
			string text2 = "[TRACE]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.Log(text2, context);
		}

		public static void Trace(string text, Object context = null)
		{
			string text2 = "[TRACE]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.Log(text2, context);
		}

		public static void Debug(string module, string text, Object context = null)
		{
			string text2 = "[DEBUG]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.Log(text2, context);
		}

		public static void Debug(string text, Object context = null)
		{
			string text2 = "[DEBUG]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.Log(text2, context);
		}

		public static void Warning(string module, string text, Object context = null)
		{
			string text2 = "[WARN]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.LogWarning(text2, context);
		}

		public static void Warning(string text, Object context = null)
		{
			string text2 = "[WARN]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.LogWarning(text2, context);
		}

		public static void Error(string module, string text, Object context = null)
		{
			string text2 = "[ERROR]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.LogError(text2, context);
		}

		public static void Error(string text, GameObject context = null)
		{
			string text2 = "[ERROR]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.LogError(text2, context);
		}

		private static string CustomFormat(string module, string text)
		{
			if (!StringExtensions.IsNullOrWhitespace(module))
			{
				module = "[" + module.ToUpper() + "] ";
				text = text.Replace("<!", "<i><color=blue>");
				text = text.Replace("!>", "</color></i>");
			}
			return module + text;
		}

		private const string MODULE_PREFIX = "[";

		private const string MODULE_SUFFIX = "]";
	}
}
