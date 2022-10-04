using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class SkinCommand : ConsoleCommand
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
				if (command == "skin")
				{
					this.ParseSkin(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"skin"
			};
		}

		private void ParseSkin(string command, List<string> paramList)
		{
			string command2 = "skin " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available SKIN commands:");
					base.Console.Write("list: get all the possible SKIN_IDs.");
					base.Console.Write("listunlocked: get all the unlocked SKIN_IDs.");
					base.Console.Write("get: get the current SKIN_ID.");
					base.Console.Write("set: set the current skin with a given SKIN_ID.");
					base.Console.Write("unlock [SKIN_ID | ALL]: unlocks the skin with SKIN_ID or all of them");
					base.Console.Write("lock [SKIN_ID | ALL]: locks the skin with SKIN_ID or all of them");
				}
				return;
			case "list":
				if (base.ValidateParams(command2, 0, paramList))
				{
					foreach (string str in Core.ColorPaletteManager.GetAllColorPalettesId())
					{
						base.Console.Write("Skin ID: " + str);
					}
				}
				return;
			case "listunlocked":
				if (base.ValidateParams(command2, 0, paramList))
				{
					foreach (string str2 in Core.ColorPaletteManager.GetAllUnlockedColorPalettesId())
					{
						base.Console.Write("Skin UNLOCKED ID: " + str2);
					}
				}
				return;
			case "get":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Current Skin ID: " + Core.ColorPaletteManager.GetCurrentColorPaletteId());
				}
				return;
			case "set":
				if (base.ValidateParams(command2, 1, paramList))
				{
					string text = paramList[0].ToUpper();
					if (Core.ColorPaletteManager.GetAllColorPalettesId().Contains(text))
					{
						Core.ColorPaletteManager.SetCurrentColorPaletteId(text);
					}
					else
					{
						base.Console.Write("Skin ID '" + text + "' doesn't exist.");
					}
				}
				return;
			case "unlock":
				if (base.ValidateParams(command2, 1, paramList))
				{
					string text2 = paramList[0].ToUpperInvariant();
					if (text2 == "ALL")
					{
						SkinCommand.UnlockAllColorPalettes();
					}
					else if (Core.ColorPaletteManager.GetAllColorPalettesId().Contains(text2))
					{
						Core.ColorPaletteManager.UnlockColorPalette(text2, true);
						base.Console.Write("Skin with ID: " + text2 + " has been unlocked.");
					}
					else
					{
						base.Console.Write("A skin with ID: " + text2 + " doesn't exist!");
					}
				}
				return;
			case "lock":
				if (base.ValidateParams(command2, 1, paramList))
				{
					string text3 = paramList[0].ToUpperInvariant();
					if (text3 == "ALL")
					{
						SkinCommand.LockAllColorPalettes();
					}
					else if (Core.ColorPaletteManager.GetAllColorPalettesId().Contains(text3))
					{
						Core.ColorPaletteManager.LockColorPalette(text3);
						base.Console.Write("Skin with ID: " + text3 + " has been locked.");
					}
					else
					{
						base.Console.Write("A skin with ID: " + text3 + " doesn't exist!");
					}
				}
				return;
			}
			base.Console.Write("Command unknow, use skin help");
		}

		private static void LockAllColorPalettes()
		{
			foreach (string colorPaletteId in from palette in Core.ColorPaletteManager.GetAllColorPalettesId()
			where palette != "PENITENT_DEFAULT"
			select palette)
			{
				Core.ColorPaletteManager.LockColorPalette(colorPaletteId);
			}
		}

		private static void UnlockAllColorPalettes()
		{
			string currentColorPaletteId = Core.ColorPaletteManager.GetCurrentColorPaletteId();
			foreach (string colorPaletteId in Core.ColorPaletteManager.GetAllColorPalettesId())
			{
				Core.ColorPaletteManager.UnlockColorPalette(colorPaletteId, false);
			}
			Core.ColorPaletteManager.SetCurrentSkinToSkinSettings(currentColorPaletteId);
		}
	}
}
