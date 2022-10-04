using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class AlmsCommand : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "alms")
				{
					this.ParseAlms(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"alms"
			};
		}

		private void ParseAlms(string command, List<string> paramList)
		{
			int tears = 0;
			string command2 = "alms " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available ALMS commands:");
						base.Console.Write("alms current: Get current alms and tier");
						base.Console.Write("alms list: List all tiers requeriments");
						base.Console.Write("alms consume NUMBER: Consume NUMBER of tears");
						base.Console.Write("alms set NUMBER: Set the current alms");
					}
					return;
				}
				if (command == "current")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Current tears given: " + Core.Alms.TearsGiven.ToString());
						base.Console.Write("Current tier: " + Core.Alms.CurentTier.ToString());
						base.Console.Write("Altar level: " + Core.Alms.GetAltarLevel().ToString());
						base.Console.Write("PrieDieu level: " + Core.Alms.GetPrieDieuLevel().ToString());
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Tears to reach tier:");
						int num = 1;
						foreach (int num2 in Core.Alms.Config.GetTearsList())
						{
							base.Console.Write(string.Concat(new string[]
							{
								"Tier ",
								num.ToString(),
								": ",
								num2.ToString(),
								" tears needed."
							}));
							num++;
						}
					}
					return;
				}
				if (command == "consume")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out tears, 0, 99999))
					{
						if (!Core.Alms.CanConsumeTears(tears))
						{
							base.Console.Write("Error can't consume " + tears.ToString() + " tears");
						}
						else
						{
							Core.Alms.ConsumeTears(tears);
							base.Console.Write("Tears consumed.");
						}
					}
					return;
				}
				if (command == "set")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out tears, 0, 99999))
					{
						Core.Alms.DEBUG_SetTearsGiven(tears);
						base.Console.Write("Alms setted");
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use audio help");
		}
	}
}
