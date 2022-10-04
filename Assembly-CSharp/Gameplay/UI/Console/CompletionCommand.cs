using System;
using System.Collections.Generic;
using Framework.Managers;
using Framework.Util.CompletionCommandHelper;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class CompletionCommand : ConsoleCommand
	{
		public override void Start()
		{
			this.gameData = Resources.Load<CompletionAssets>("Game Completion Data/Completion Data");
		}

		public override string GetName()
		{
			return "completion";
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "completion")
				{
					this.ParseCompletion(subcommand, paramList);
				}
			}
		}

		private void ParseCompletion(string command, List<string> paramList)
		{
			string text = command.ToUpperInvariant();
			if (text != null)
			{
				if (text == "GET" || text == "SHOW")
				{
					this.ShowCurrentCompletion();
					return;
				}
				if (text == "BASE")
				{
					this.RunUnlock(false);
					return;
				}
				if (text == "NG+")
				{
					this.RunUnlock(true);
					return;
				}
			}
			this.ShowHelp();
		}

		private float ShowCurrentCompletion()
		{
			float percentCompleted = Core.Persistence.PercentCompleted;
			base.Console.WriteFormat("Current completion: {0:00.000}%", new object[]
			{
				percentCompleted
			});
			return percentCompleted;
		}

		private void RunUnlock(bool isNGPlus)
		{
			string text = (!isNGPlus) ? "base" : "NG+";
			float num = this.ShowCurrentCompletion();
			base.Console.WriteFormat("Unlocking {0} game items...", new object[]
			{
				text
			});
			this.Run(this.gameData.UnlockBaseGame());
			if (isNGPlus)
			{
				this.Run(this.gameData.UnlockNGPlus());
			}
			base.Console.Write("... DONE");
			float num2 = this.ShowCurrentCompletion();
			base.Console.WriteFormat("TOTAL % added: {0:00.000}%", new object[]
			{
				num2 - num
			});
		}

		private void Run(IEnumerable<string> enumerable)
		{
			foreach (string message in enumerable)
			{
				base.Console.Write(message);
				this.ShowCurrentCompletion();
			}
		}

		private void ShowHelp()
		{
			base.Console.WriteFormat("Available {0} commands:", new object[]
			{
				"completion"
			});
			base.Console.WriteFormat("{0} get: Shows current completion %", new object[]
			{
				"completion"
			});
			base.Console.WriteFormat("{0} base: Unlocks 100% base game", new object[]
			{
				"completion"
			});
			base.Console.WriteFormat("{0} ng+: Unlocks 150%: 100% base game + 50% NG+", new object[]
			{
				"completion"
			});
		}

		private const string COMMAND = "completion";

		private CompletionAssets gameData;

		private const string RESOURCE_PATH = "Game Completion Data/Completion Data";
	}
}
