using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class TeleportCommand : ConsoleCommand
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
				if (command == "teleport")
				{
					this.ParseTeleport(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"teleport"
			};
		}

		private void ParseTeleport(string command, List<string> paramList)
		{
			string command2 = "teleport " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available TELEPORT commands:");
						base.Console.Write("teleport list: List all teleports ID");
						base.Console.Write("teleport go IDTELEPORT: Teleport to this teleport");
						base.Console.Write("teleport showui: Show the teleport UI");
						base.Console.Write("teleport unlock [ID|ALL]: Unlocks the teleport ID or all");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("*** All teleports:");
						foreach (TeleportDestination teleportDestination in Core.SpawnManager.GetAllTeleports())
						{
							string message = string.Concat(new string[]
							{
								teleportDestination.id,
								": ",
								teleportDestination.caption,
								"  (",
								teleportDestination.sceneName,
								", ",
								teleportDestination.teleportName,
								")"
							});
							base.Console.Write(message);
						}
					}
					return;
				}
				if (command == "go")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						Core.SpawnManager.Teleport(paramList[0].ToUpper());
					}
					return;
				}
				if (command == "showui")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						UIController.instance.ShowTeleportUI();
					}
					return;
				}
				if (command == "unlock")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						TeleportCommand.UnlockTeleport(paramList[0]);
					}
					return;
				}
			}
			base.Console.Write("Command unknown, use teleport help");
		}

		private static void UnlockTeleport(string teleportID)
		{
			string text = teleportID.ToUpperInvariant();
			if (text != null)
			{
				if (text == "ALL")
				{
					foreach (TeleportDestination teleportDestination in Core.SpawnManager.GetAllTeleports())
					{
						if (teleportDestination.useInUI)
						{
							Core.SpawnManager.SetTeleportActive(teleportDestination.id, true);
						}
					}
					return;
				}
			}
			Core.SpawnManager.SetTeleportActive(teleportID.ToUpperInvariant(), true);
		}
	}
}
