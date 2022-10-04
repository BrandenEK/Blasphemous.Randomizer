using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class GameModeCommand : ConsoleCommand
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
				if (command == "gamemode")
				{
					this.ParseGameMode(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"gamemode"
			};
		}

		private void ParseGameMode(string command, List<string> paramList)
		{
			string command2 = "gamemode " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available GAMEMODE commands:");
						base.Console.Write("list: get all the possible game modes.");
						base.Console.Write("current: get the currently active game mode.");
						base.Console.Write("set GAME_MODE: sets the currently active game mode to a given value.");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						foreach (string str in GameModeManager.GetAllGameModesNames())
						{
							base.Console.Write("Game Mode: " + str);
						}
					}
					return;
				}
				if (command == "current")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Current Game Mode: " + Core.GameModeManager.GetCurrentGameModeName());
					}
					return;
				}
				if (command == "set")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (Core.GameModeManager.GameModeExists(paramList[0]))
						{
							Core.GameModeManager.ChangeMode(paramList[0]);
							base.Console.Write("Game Mode has been changed to: " + paramList[0].ToUpper());
						}
						else
						{
							base.Console.Write("A Game Mode with name: " + paramList[0].ToUpper() + " doesn't exist!");
						}
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use gamemode help");
		}
	}
}
