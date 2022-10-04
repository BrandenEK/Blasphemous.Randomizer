using System;
using System.Collections.Generic;
using Framework.FrameworkCore.Attributes.Logic;
using Gameplay.UI.Widgets;

namespace Gameplay.UI.Console
{
	public class StatsCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> list;
			string subcommand = base.GetSubcommand(parameters, out list);
			Framework.FrameworkCore.Attributes.Logic.Attribute attribute = null;
			VariableAttribute variableAttribute = null;
			switch (command)
			{
			case "health":
				variableAttribute = base.Penitent.Stats.Life;
				break;
			case "flask":
				variableAttribute = base.Penitent.Stats.Flask;
				break;
			case "fervour":
				variableAttribute = base.Penitent.Stats.Fervour;
				break;
			case "purge":
				variableAttribute = base.Penitent.Stats.Purge;
				break;
			case "meaculpa":
				attribute = base.Penitent.Stats.MeaCulpa;
				break;
			case "strength":
				attribute = base.Penitent.Stats.Strength;
				break;
			case "flaskhealth":
				attribute = base.Penitent.Stats.FlaskHealth;
				break;
			}
			if (attribute == null)
			{
				attribute = variableAttribute;
			}
			if (subcommand == "help")
			{
				base.Console.Write("Available " + command + " commands:");
				base.Console.Write(command + " current: Show current value");
				base.Console.Write(command + " set VALUE: Set current value");
				base.Console.Write(command + " reset: Reset the upgrades");
				base.Console.Write(command + " upgrade: Upgrade stat");
				base.Console.Write(command + " upgradeto VALUE: Upgrade stat until final value is less that NUMBER");
				if (attribute.IsVariable())
				{
					base.Console.Write(command + " fill: Fill to max value");
					base.Console.Write(command + " setmax VALUE: Set max value");
				}
				return;
			}
			if (!attribute.IsVariable())
			{
				if (subcommand != null)
				{
					if (subcommand == "current")
					{
						base.Console.Write(string.Concat(new string[]
						{
							"Current: ",
							attribute.Final.ToString(),
							" (perma:",
							attribute.PermanetBonus.ToString(),
							")"
						}));
						goto IL_4F3;
					}
					if (subcommand == "set")
					{
						float num2;
						if (base.ValidateParams(command + " set", 1, list) && base.ValidateParam(list[0], out num2, 0f, 99999f))
						{
							attribute.ConsoleSet(num2);
							base.WriteCommandResult(command + " set", true);
						}
						goto IL_4F3;
					}
					if (subcommand == "upgrade")
					{
						attribute.Upgrade();
						base.Console.Write(string.Concat(new string[]
						{
							"Upgraded, new value: ",
							attribute.Final.ToString(),
							" (perma:",
							attribute.PermanetBonus.ToString(),
							")"
						}));
						goto IL_4F3;
					}
					if (subcommand == "reset")
					{
						attribute.ResetUpgrades();
						base.Console.Write(string.Concat(new string[]
						{
							"Reset, new value: ",
							attribute.Final.ToString(),
							" (perma:",
							attribute.PermanetBonus.ToString(),
							")"
						}));
						goto IL_4F3;
					}
					if (subcommand == "upgradeto")
					{
						int num3;
						if (base.ValidateParams(command + " upgradeto", 1, list) && base.ValidateParam(list[0], out num3, 0, 9999))
						{
							while (attribute.Final < (float)num3)
							{
								attribute.Upgrade();
							}
							base.Console.Write(string.Concat(new string[]
							{
								"Upgraded, new value: ",
								attribute.Final.ToString(),
								" (perma:",
								attribute.PermanetBonus.ToString(),
								")"
							}));
						}
						goto IL_4F3;
					}
				}
				base.Console.Write("Command unknow, use " + command + " help");
				IL_4F3:;
			}
			else
			{
				switch (subcommand)
				{
				case "current":
				{
					ConsoleWidget console = base.Console;
					string[] array = new string[8];
					array[0] = "Current: ";
					int num4 = 1;
					float num5 = variableAttribute.Current;
					array[num4] = num5.ToString();
					array[2] = ", ATTR_MAX:";
					array[3] = variableAttribute.Final.ToString();
					array[4] = " (perma:";
					array[5] = variableAttribute.PermanetBonus.ToString();
					array[6] = "), MAX:";
					array[7] = variableAttribute.MaxValue.ToString();
					console.Write(string.Concat(array));
					return;
				}
				case "fill":
					variableAttribute.SetToCurrentMax();
					base.WriteCommandResult(command + " fill", true);
					return;
				case "set":
				{
					float num2;
					if (base.ValidateParams(command + " set", 1, list) && base.ValidateParam(list[0], out num2, 0f, 99999f))
					{
						variableAttribute.Current = num2;
						base.WriteCommandResult(command + " set", true);
					}
					return;
				}
				case "setmax":
				{
					float num2;
					if (base.ValidateParams(command + " setmax", 1, list) && base.ValidateParam(list[0], out num2, 0f, 99999f))
					{
						variableAttribute.SetPermanentBonus(0f);
						variableAttribute.MaxValue = num2;
						base.WriteCommandResult(command + " setmax", true);
					}
					return;
				}
				case "upgrade":
					variableAttribute.Upgrade();
					base.Console.Write(string.Concat(new string[]
					{
						"Upgraded, new value: ",
						attribute.Final.ToString(),
						" (perma:",
						attribute.PermanetBonus.ToString(),
						")"
					}));
					return;
				case "reset":
					attribute.ResetUpgrades();
					base.Console.Write(string.Concat(new string[]
					{
						"Reset, new value: ",
						attribute.Final.ToString(),
						" (perma:",
						attribute.PermanetBonus.ToString(),
						")"
					}));
					return;
				case "upgradeto":
				{
					int num3;
					if (base.ValidateParams(command + " upgradeto", 1, list) && base.ValidateParam(list[0], out num3, 0, 9999))
					{
						while (attribute.Final < (float)num3)
						{
							attribute.Upgrade();
						}
						base.Console.Write(string.Concat(new string[]
						{
							"Upgraded, new value: ",
							attribute.Final.ToString(),
							" (perma:",
							attribute.PermanetBonus.ToString(),
							")"
						}));
					}
					return;
				}
				}
				base.Console.Write("Command unknow, use " + command + " help");
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"health",
				"flask",
				"fervour",
				"purge",
				"meaculpa",
				"strength",
				"flaskhealth"
			};
		}
	}
}
