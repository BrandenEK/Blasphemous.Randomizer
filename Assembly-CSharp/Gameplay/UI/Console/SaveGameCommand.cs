using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class SaveGameCommand : ConsoleCommand
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
				if (command == "savegame")
				{
					this.ParseSaveGame(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"savegame"
			};
		}

		private void ParseSaveGame(string command, List<string> paramList)
		{
			string command2 = "savegame " + command;
			int slot = 0;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available SAVEGAME commands:");
						base.Console.Write("savegame load SLOT: Loads the current game to slot");
						base.Console.Write("savegame save SLOT: Save the current game to slot");
						base.Console.Write("savegame enablenewgameplus: Enable new game plus in this slot.");
					}
					return;
				}
				if (command == "load")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out slot, 0, 10))
					{
						if (Core.Persistence.ExistSlot(slot))
						{
							Core.Persistence.LoadGame(slot);
						}
						else
						{
							base.Console.Write("Slot " + slot.ToString() + " not found");
						}
					}
					return;
				}
				if (command == "save")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out slot, 0, 10))
					{
						Core.Persistence.SaveGame(slot, true);
					}
					return;
				}
				if (command == "enablenewgameplus")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.Persistence.HACK_EnableNewGamePlusInCurrent();
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use savegame help");
		}
	}
}
