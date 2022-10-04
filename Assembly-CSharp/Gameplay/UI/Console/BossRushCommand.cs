using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class BossRushCommand : ConsoleCommand
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
				if (command == "bossrush")
				{
					this.ParseBossRush(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"bossrush"
			};
		}

		private void ParseBossRush(string command, List<string> paramList)
		{
			string command2 = "bossrush " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available BOSS RUSH commands:");
					base.Console.Write("start course_x y: starts the x boss rush course with the difficulty y.");
					base.Console.Write("hub: loads the hub scene.");
					base.Console.Write("next: loads the next course scene.");
					base.Console.Write("golast: goes to last boss.");
					base.Console.Write("end: ends the current boss rush run if any, showing the results.");
					base.Console.Write("printscore: prints in the console the current score.");
					base.Console.Write("unlock course_x: unlocks course_x.");
				}
				return;
			case "start":
				if (base.ValidateParams(command2, 2, paramList))
				{
					string value = paramList[0];
					string value2 = paramList[1];
					BossRushManager.BossRushCourseId courseId = (BossRushManager.BossRushCourseId)Enum.Parse(typeof(BossRushManager.BossRushCourseId), value, true);
					BossRushManager.BossRushCourseMode courseMode = (BossRushManager.BossRushCourseMode)Enum.Parse(typeof(BossRushManager.BossRushCourseMode), value2, true);
					Core.BossRushManager.StartCourse(courseId, courseMode, -1);
				}
				return;
			case "hub":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.BossRushManager.LoadHub(true);
				}
				return;
			case "next":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.BossRushManager.LoadCourseNextScene();
				}
				return;
			case "golast":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.BossRushManager.LoadLastScene();
				}
				return;
			case "end":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.BossRushManager.EndCourse(true);
				}
				return;
			case "printscore":
				if (base.ValidateParams(command2, 0, paramList))
				{
					Core.BossRushManager.LogHighScoreObtained();
				}
				return;
			case "unlock":
				if (base.ValidateParams(command2, 1, paramList))
				{
					string courseId2 = paramList[0].ToUpper();
					Core.BossRushManager.DEBUGUnlockCourse(courseId2);
				}
				return;
			}
			base.Console.Write("Command unknow, use bossrush help");
		}
	}
}
