using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class FlagCommand : ConsoleCommand
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
				if (command == "flag")
				{
					this.ParseFlag(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"flag"
			};
		}

		private void ParseFlag(string command, List<string> paramList)
		{
			string command2 = "dialog " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available FLAG commands:");
						base.Console.Write("flag set IDFLAG: Set flag to true");
						base.Console.Write("flag clear IDFLAG: Set flag to false");
						base.Console.Write("flag test IDFLAG: Outputs the current value of flag");
					}
					return;
				}
				if (command == "set")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						Core.Events.SetFlag(paramList[0].Trim(), true, false);
						base.Console.Write("Flag: " + paramList[0].Trim() + " set to true.");
					}
					return;
				}
				if (command == "clear")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						Core.Events.SetFlag(paramList[0].Trim(), false, false);
						base.Console.Write("Flag: " + paramList[0].Trim() + " set to false.");
					}
					return;
				}
				if (command == "test")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (Core.Events.GetFlag(paramList[0].Trim()))
						{
							base.Console.Write("Flag: " + paramList[0].Trim() + " is: true.");
						}
						else
						{
							base.Console.Write("Flag: " + paramList[0].Trim() + " is: false.");
						}
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use flag help");
		}
	}
}
