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

		public static void Trace(string module, string text, UnityEngine.Object context = null)
		{
			string message = "[TRACE]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.Log(message, context);
		}

		public static void Trace(string text, UnityEngine.Object context = null)
		{
			string message = "[TRACE]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.Log(message, context);
		}

		public static void Debug(string module, string text, UnityEngine.Object context = null)
		{
			string message = "[DEBUG]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.Log(message, context);
		}

		public static void Debug(string text, UnityEngine.Object context = null)
		{
			string message = "[DEBUG]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.Log(message, context);
		}

		public static void Warning(string module, string text, UnityEngine.Object context = null)
		{
			string message = "[WARN]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.LogWarning(message, context);
		}

		public static void Warning(string text, UnityEngine.Object context = null)
		{
			string message = "[WARN]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.LogWarning(message, context);
		}

		public static void Error(string module, string text, UnityEngine.Object context = null)
		{
			string message = "[ERROR]" + Log.CustomFormat(module, text);
			UnityEngine.Debug.LogError(message, context);
		}

		public static void Error(string text, GameObject context = null)
		{
			string message = "[ERROR]" + Log.CustomFormat(string.Empty, text);
			UnityEngine.Debug.LogError(message, context);
		}

		private static string CustomFormat(string module, string text)
		{
			if (!module.IsNullOrWhitespace())
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
