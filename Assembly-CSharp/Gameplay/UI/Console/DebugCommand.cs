using System;
using System.Collections.Generic;
using Framework.Managers;
using Framework.Util;

namespace Gameplay.UI.Console
{
	public class DebugCommand : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "debug")
				{
					this.ParseDebug(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"debug"
			};
		}

		private void ParseDebug(string command, List<string> paramList)
		{
			string command2 = "debug " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available DEBUG commands:");
						base.Console.Write("debug list: List all debug systems");
						base.Console.Write("debug on SYSTEM: Turn on debug for this system");
						base.Console.Write("debug off SYSTEM: Turn off debug for this system");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("All systems ID:");
						foreach (string message in Singleton<Core>.Instance.GetSystemsId())
						{
							base.Console.Write(message);
						}
					}
					return;
				}
				if (command == "on")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						Singleton<Core>.Instance.SetDebug(paramList[0], true);
						base.Console.Write("Setting on " + paramList[0]);
					}
					return;
				}
				if (command == "off")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						Singleton<Core>.Instance.SetDebug(paramList[0], false);
						base.Console.Write("Setting off " + paramList[0]);
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use debug help");
		}
	}
}
