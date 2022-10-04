using System;

namespace Gameplay.UI.Console
{
	public class Help : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			if (parameters.Length == 0)
			{
				base.Console.Write("Available commands:");
				string text = string.Empty;
				int num = 130;
				foreach (string str in base.Console.GetAllNames())
				{
					text = text + str + ", ";
					if (text.Length > num)
					{
						base.Console.Write(text);
						text = string.Empty;
					}
				}
				if (text != string.Empty)
				{
					base.Console.Write(text);
				}
			}
			else if (parameters[0].ToUpper() != "HELP")
			{
				base.Console.ProcessCommand(parameters[0] + " help");
			}
			else
			{
				base.Console.Write("Use help COMMAND to get help");
			}
		}

		public override string GetName()
		{
			return "help";
		}
	}
}
