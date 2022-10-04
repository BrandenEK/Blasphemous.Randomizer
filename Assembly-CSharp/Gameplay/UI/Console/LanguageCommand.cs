using System;
using System.Collections.Generic;
using I2.Loc;

namespace Gameplay.UI.Console
{
	public class LanguageCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> list;
			string subcommand = base.GetSubcommand(parameters, out list);
			if (subcommand != null)
			{
				if (subcommand == "help")
				{
					base.Console.Write("Available LANGUAGE commands:");
					base.Console.Write("language list: List all available languages");
					base.Console.Write("language current: Show the current language");
					base.Console.Write("language set LANGUAGE_CODE: Sets the language");
					return;
				}
				if (subcommand == "list")
				{
					base.Console.Write("The available languages are:");
					foreach (string text in LocalizationManager.GetAllLanguagesCode(true, true))
					{
						string languageFromCode = LocalizationManager.GetLanguageFromCode(text, true);
						base.Console.Write(text + ": " + languageFromCode);
					}
					return;
				}
				if (subcommand == "current")
				{
					base.Console.Write("The current language is " + LocalizationManager.CurrentLanguage + " with code " + LocalizationManager.CurrentLanguageCode);
					return;
				}
				if (subcommand == "set")
				{
					if (base.ValidateParams(subcommand, 1, list))
					{
						string languageFromCode2 = LocalizationManager.GetLanguageFromCode(list[0], true);
						string supportedLanguage = LocalizationManager.GetSupportedLanguage(languageFromCode2);
						if (string.IsNullOrEmpty(supportedLanguage))
						{
							base.Console.Write("language set: Language code " + list[0] + " not supported, use language list");
						}
						else
						{
							LocalizationManager.CurrentLanguage = supportedLanguage;
							base.Console.Write("Language setted");
						}
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use language help");
		}

		public override string GetName()
		{
			return "language";
		}
	}
}
