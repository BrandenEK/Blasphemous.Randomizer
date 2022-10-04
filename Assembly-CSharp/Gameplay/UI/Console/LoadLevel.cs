using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class LoadLevel : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			List<string> list;
			string subcommand = base.GetSubcommand(parameters, out list);
			if (command != null)
			{
				if (!(command == "load"))
				{
					if (!(command == "loadmenu"))
					{
						if (command == "loadnoui")
						{
							UIController.instance.HideAllNotInGameUI();
							Core.SpawnManager.PrepareForCommandSpawn(subcommand.ToUpper());
							Core.LevelManager.ChangeLevel(subcommand.ToUpper(), false, true, true, null);
						}
					}
					else
					{
						Core.Logic.LoadMenuScene(false);
					}
				}
				else
				{
					LevelManager.OnLevelLoaded += this.OnLevelLoaded;
					string levelName = subcommand.ToUpper();
					Core.LevelManager.ChangeLevel(levelName, false, true, true, null);
				}
			}
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			Core.UI.NavigationUI.Show(true);
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"load",
				"loadmenu",
				"loadnoui"
			};
		}

		public override bool HasLowerParameters()
		{
			return false;
		}
	}
}
