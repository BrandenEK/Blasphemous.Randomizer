using System;
using System.Collections.Generic;
using Framework.Managers;
using Steamworks;

namespace Gameplay.UI.Console
{
	public class AchievementCommand : ConsoleCommand
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
				if (command == "achievement")
				{
					this.ParseAchievement(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"achievement"
			};
		}

		private void ParseAchievement(string command, List<string> paramList)
		{
			string command2 = "achievement " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available ACHIEVEMENT commands:");
					base.Console.Write("enablepopup: Show the popup when achievement is granted");
					base.Console.Write("disablepopup: Dont show the popup when achievement is granted");
					base.Console.Write("check acxx: checks whether the achievement with acxx as its ID is granted or not.");
					base.Console.Write("grant acxx: grants the achievement with acxx as its ID.");
					base.Console.Write("clear acxx: clears the progress of the achievement with acxx as its ID.");
					base.Console.Write("clearsteam acxx: clears the progress of the achievement with acxx on Steam.");
					base.Console.Write("clearall: clears the progress of all the achievements.");
					base.Console.Write("clearsteamall: clears the progress of all the achievements on Steam.");
					base.Console.Write("addprogress acxx progress: adds progress (from 0f to 100f) to the achievement with acxx as its ID.");
					base.Console.Write("checkprogress acxx: checks the progress of the achievement with acxx as its ID.");
				}
				return;
			case "enablepopup":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.AchievementsManager.ShowPopUp = true;
					base.Console.Write("Popup is enabled");
				}
				return;
			case "disablepopup":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.AchievementsManager.ShowPopUp = false;
					base.Console.Write("Popup is disabled");
				}
				return;
			case "check":
				if (base.ValidateParams(command2, 1, paramList))
				{
					bool flag = Core.AchievementsManager.CheckAchievementGranted(paramList[0]);
					if (flag)
					{
						base.Console.Write("Achievement: " + paramList[0] + " is granted.");
					}
					else
					{
						base.Console.Write("Achievement: " + paramList[0] + " is not granted.");
					}
				}
				return;
			case "grant":
				if (base.ValidateParams(command2, 1, paramList))
				{
					bool flag2 = Core.AchievementsManager.CheckAchievementGranted(paramList[0]);
					if (flag2)
					{
						base.Console.Write("Achievement: " + paramList[0] + " is already granted.");
					}
					else
					{
						Core.AchievementsManager.GrantAchievement(paramList[0]);
						flag2 = Core.AchievementsManager.CheckAchievementGranted(paramList[0]);
						if (flag2)
						{
							base.Console.Write("Achievement: " + paramList[0] + " has been granted.");
						}
						else
						{
							base.Console.Write("Achievement: " + paramList[0] + " hasn't been granted. Something went wrong.");
						}
					}
				}
				return;
			case "clear":
				if (base.ValidateParams(command2, 1, paramList))
				{
					Core.AchievementsManager.DebugResetAchievement(paramList[0]);
					bool flag3 = Core.AchievementsManager.CheckAchievementGranted(paramList[0]);
					if (flag3)
					{
						base.Console.Write("Achievement: " + paramList[0] + " hasn't been cleared. Something went wrong.");
					}
					else
					{
						base.Console.Write("Achievement: " + paramList[0] + " has been cleared.");
					}
				}
				return;
			case "clearsteam":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (SteamManager.Initialized)
					{
						bool flag4 = SteamUserStats.ClearAchievement(paramList[0]);
						if (!SteamUserStats.StoreStats())
						{
							base.Console.Write(string.Concat(new object[]
							{
								"Achievement: ",
								paramList[0],
								" hasn't been cleared from Steam for user ",
								SteamUser.GetHSteamUser().m_HSteamUser,
								". Something went wrong."
							}));
						}
						else
						{
							base.Console.Write(string.Concat(new object[]
							{
								"Achievement: ",
								paramList[0],
								" has been cleared from Steam for user ",
								SteamUser.GetHSteamUser().m_HSteamUser
							}));
						}
					}
					else
					{
						base.Console.Write("STEAM is not initialized!");
					}
				}
				return;
			case "clearall":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.AchievementsManager.DebugReset();
				}
				return;
			case "clearsteamall":
				if (base.ValidateParams(command2, 0, paramList))
				{
					if (SteamManager.Initialized)
					{
						bool flag5 = false;
						foreach (string text in Core.AchievementsManager.Achievements.Keys)
						{
							flag5 |= SteamUserStats.ClearAchievement(text);
						}
						if (!SteamUserStats.StoreStats())
						{
							base.Console.Write("Achievements hasn't been cleared from Steam. Something went wrong.");
						}
						else
						{
							base.Console.Write("Achievements has been cleared from Steam for user " + SteamUser.GetHSteamUser().m_HSteamUser);
						}
					}
					else
					{
						base.Console.Write("STEAM is not initialized!");
					}
				}
				return;
			case "addprogress":
				if (base.ValidateParams(command2, 2, paramList))
				{
					Core.AchievementsManager.AddAchievementProgress(paramList[0], float.Parse(paramList[1]));
					base.Console.Write("Achievement: " + paramList[0] + " has been added a progress of: " + paramList[1]);
				}
				return;
			case "checkprogress":
				if (base.ValidateParams(command2, 1, paramList))
				{
					float num2 = Core.AchievementsManager.CheckAchievementProgress(paramList[0]);
					base.Console.Write(string.Concat(new object[]
					{
						"Achievement: ",
						paramList[0],
						" has a progress of: ",
						num2
					}));
				}
				return;
			}
			base.Console.Write("Command unknow, use achievement help");
		}
	}
}
