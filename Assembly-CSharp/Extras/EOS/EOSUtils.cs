using System;
using System.Linq;

namespace Extras.EOS
{
	public static class EOSUtils
	{
		public static T GetBestValue<T>(T fallbackValue, string argName)
		{
			string text = EOSUtils.GetCommandLineArgValue(argName);
			text = text.Trim();
			int num;
			if (fallbackValue.GetType().IsEnum && !string.IsNullOrEmpty(text) && int.TryParse(text, out num) && Enum.IsDefined(fallbackValue.GetType(), num))
			{
				return (T)((object)num);
			}
			return fallbackValue;
		}

		public static string GetBestValue(string fallbackValue, string argName)
		{
			string text = EOSUtils.GetCommandLineArgValue(argName);
			text = text.Trim();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return fallbackValue;
		}

		public static string GetCommandLineArgValue(string argName)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i].StartsWith(argName))
				{
					string[] array = commandLineArgs[i].Split(new char[]
					{
						'='
					});
					if (array.Length > 1)
					{
						return array[1];
					}
				}
			}
			return string.Empty;
		}

		public static bool HasCommandLineArg(string argName)
		{
			return Environment.GetCommandLineArgs().Contains(argName);
		}
	}
}
