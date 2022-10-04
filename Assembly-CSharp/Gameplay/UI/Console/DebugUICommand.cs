using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class DebugUICommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "show_debug_ui")
				{
					this.ParseShowUI(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"show_debug_ui"
			};
		}

		private void ParseShowUI(string command, List<string> paramList)
		{
			string command2 = "show_debug_ui " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available show_debug_ui commands:");
						base.Console.Write("show_debug_ui off: Turn off DEBUG UI");
						base.Console.Write("show_debug_ui on: Turn on DEBUG UI");
						base.Console.Write("show_debug_ui current: Show the current option");
					}
					return;
				}
				if (command == "current")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("*** Current Debug UI: " + Core.UI.ConsoleShowDebugUI.ToString());
					}
					return;
				}
				if (command == "on")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.UI.ConsoleShowDebugUI = true;
					}
					return;
				}
				if (command == "off")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.UI.ConsoleShowDebugUI = false;
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use show_debug_ui help");
		}
	}
}
