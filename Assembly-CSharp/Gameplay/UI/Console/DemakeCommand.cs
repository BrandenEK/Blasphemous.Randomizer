using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class DemakeCommand : ConsoleCommand
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
				if (command == "demake")
				{
					this.ParseDemake(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"demake"
			};
		}

		private void ParseDemake(string command, List<string> paramList)
		{
			string command2 = "demake " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available DEMAKE commands:");
						base.Console.Write("enter: enter the demake.");
					}
					return;
				}
				if (command == "enter")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						Core.DemakeManager.StartDemakeRun();
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use demake help");
		}
	}
}
