using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class ShowUICommand : ConsoleCommand
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
				if (command == "showui")
				{
					this.ParseShowUI(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"showui"
			};
		}

		private void ParseShowUI(string command, List<string> paramList)
		{
			string command2 = "showui " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available SHOWUI commands:");
						base.Console.Write("showui off: Turn off UI");
						base.Console.Write("showui on: Turn on UI");
						base.Console.Write("showui current: Show the current option");
					}
					return;
				}
				if (command == "current")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("*** Current ShowUI: " + Core.UI.ShowGamePlayUIForDebug.ToString());
					}
					return;
				}
				if (command == "on")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.UI.ShowGamePlayUIForDebug = true;
					}
					return;
				}
				if (command == "off")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.UI.ShowGamePlayUIForDebug = false;
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use showui help");
		}
	}
}
