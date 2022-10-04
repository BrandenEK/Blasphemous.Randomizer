using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class GuiltCommand : ConsoleCommand
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
				if (command == "guilt")
				{
					this.ParseGuilt(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"guilt"
			};
		}

		private void ParseGuilt(string command, List<string> paramList)
		{
			string command2 = "guilt " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available GUILT commands:");
						base.Console.Write("guilt get: Get the level of guilt");
						base.Console.Write("guilt reset: Reset guilt to 0");
						base.Console.Write("guilt add: Add guilt to current position");
					}
					return;
				}
				if (command == "get")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Current guilt: " + Core.GuiltManager.GetDropsCount());
					}
					return;
				}
				if (command == "reset")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.GuiltManager.ResetGuilt(true);
						base.Console.Write("Guilt reset to 0.");
					}
					return;
				}
				if (command == "add")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.GuiltManager.AddGuilt();
						base.Console.Write("Guilt added.");
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use guilt help");
		}
	}
}
