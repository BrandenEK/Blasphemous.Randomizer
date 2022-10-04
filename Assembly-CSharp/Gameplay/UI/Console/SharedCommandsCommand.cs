using System;
using System.Collections.Generic;
using Framework.Managers;
using Tools.DataContainer;

namespace Gameplay.UI.Console
{
	public class SharedCommandsCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public bool ExecuteIfIsCommand(string command)
		{
			return Core.SharedCommands.ExecuteCommand(command);
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "command")
				{
					this.ParseCommand(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"command"
			};
		}

		private void ParseCommand(string command, List<string> paramList)
		{
			string command2 = "command " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available COMMAND subcommands:");
						base.Console.Write("command help: This help");
						base.Console.Write("command list: Show all commands");
						base.Console.Write("command refresh: Reload all commands");
						base.Console.Write("command IDCOMMAND: Execute IDCOMMAND command");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("All shared commands");
						foreach (SharedCommand sharedCommand in Core.SharedCommands.GetAllCommands())
						{
							base.Console.Write(sharedCommand.Id + " :" + sharedCommand.Description);
						}
					}
					return;
				}
				if (command == "refresh")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.SharedCommands.RefreshCommands();
						base.Console.Write("Commands refreshed");
					}
					return;
				}
			}
			base.WriteCommandResult("Executing " + command, Core.SharedCommands.ExecuteCommand(command));
		}
	}
}
