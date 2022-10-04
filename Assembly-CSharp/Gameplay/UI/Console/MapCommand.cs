using System;
using System.Collections.Generic;
using Framework.Managers;
using Framework.Map;
using Gameplay.UI.Widgets;

namespace Gameplay.UI.Console
{
	public class MapCommand : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "map")
				{
					this.ParseMap(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"map"
			};
		}

		public override bool HasLowerParameters()
		{
			return false;
		}

		public override bool ToLowerAll()
		{
			return false;
		}

		private void ParseMap(string command, List<string> paramList)
		{
			string command2 = "map " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available MAP commands:");
					base.Console.Write("map list: List all maps.");
					base.Console.Write("map set MAPID: Set current map.");
					base.Console.Write("map secrets: List currentmap secrets.");
					base.Console.Write("map secret SECRETID ON/OFF: Sets the secret SECRETID on or off in current map.");
					base.Console.Write("map reveal all: Reveal all map");
					base.Console.Write("map reveal DISTRICT: Reveal district DISTRICT");
					base.Console.Write("map reveal DISTRICT ZONE: Reveal zone ZONE in district DISTRICT");
					base.Console.Write("map unrevealed: Lists all unrevealed cells in current map");
				}
				return;
			case "list":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available MAPs:");
					foreach (string text in Core.NewMapManager.GetAllMaps())
					{
						string text2 = text;
						if (Core.NewMapManager.GetCurrentMap() == text)
						{
							text2 += "  <--- Current";
						}
						base.Console.Write(text2);
					}
				}
				return;
			case "set":
				if (base.ValidateParams(command2, 1, paramList))
				{
					base.WriteCommandResult("map set", Core.NewMapManager.SetCurrentMap(paramList[0]));
				}
				return;
			case "secrets":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available Secrets:");
					foreach (SecretData secretData in Core.NewMapManager.GetAllSecrets())
					{
						base.Console.Write(secretData.Name + ", revealed: " + secretData.Revealed.ToString());
					}
				}
				return;
			case "secret":
				if (base.ValidateParams(command2, 2, paramList))
				{
					string secretId = paramList[0];
					bool enable = paramList[1].ToUpper() == "ON" || paramList[1].ToUpper() == "TRUE";
					base.WriteCommandResult("map secret", Core.NewMapManager.SetSecret(secretId, enable));
				}
				return;
			case "unrevealed":
			{
				base.Console.WriteFormat("Map type: {0}", new object[]
				{
					(!Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS)) ? "base" : "NG+"
				});
				List<CellData> unrevealedCellsForCompletion = Core.NewMapManager.GetUnrevealedCellsForCompletion();
				base.Console.WriteFormat("Total unrevealed cells: {0}", new object[]
				{
					unrevealedCellsForCompletion.Count
				});
				foreach (CellData cellData in unrevealedCellsForCompletion)
				{
					base.Console.WriteFormat("   Cell: {0} {1}", new object[]
					{
						cellData.ZoneId.GetKey(),
						cellData.CellKey
					});
				}
				return;
			}
			case "reveal":
				if (paramList.Count != 1 && paramList.Count != 2)
				{
					ConsoleWidget console = base.Console;
					string str = "The command map reveal needs 1 or 2 params. You passed ";
					int count = paramList.Count;
					console.Write(str + count.ToString());
				}
				else if (paramList.Count == 1)
				{
					if (paramList[0].ToUpper() == "ALL")
					{
						Core.NewMapManager.RevealAllMap();
					}
					else if (paramList[0].ToUpper() == "NG")
					{
						Core.NewMapManager.RevealAllNGMap();
					}
					else
					{
						Core.NewMapManager.RevealAllDistrict(paramList[0]);
					}
				}
				else
				{
					Core.NewMapManager.RevealAllZone(paramList[0], paramList[1]);
				}
				return;
			}
			base.Console.Write("Command unknow, use map help");
		}
	}
}
