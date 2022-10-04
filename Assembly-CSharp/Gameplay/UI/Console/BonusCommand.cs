using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Framework.FrameworkCore.Attributes.Logic;
using Gameplay.GameControllers.Entities;

namespace Gameplay.UI.Console
{
	public class BonusCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> list;
			string subcommand = base.GetSubcommand(parameters, out list);
			if (subcommand == "help")
			{
				base.Console.Write("Available " + command + " commands:");
				base.Console.Write(command + " help: Show this help");
				base.Console.Write(command + " list: List all bonuses");
				return;
			}
			if (subcommand != null)
			{
				if (subcommand == "list")
				{
					base.Console.Write("Current bonuses:");
					IEnumerator enumerator = Enum.GetValues(typeof(EntityStats.StatsTypes)).GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							EntityStats.StatsTypes nameType = (EntityStats.StatsTypes)obj;
							Framework.FrameworkCore.Attributes.Logic.Attribute byType = base.Penitent.Stats.GetByType(nameType);
							ReadOnlyCollection<RawBonus> rawBonus = byType.GetRawBonus();
							if (rawBonus.Count > 0 || byType.PermanetBonus > 0f)
							{
								base.Console.Write(nameType.ToString() + " stat, permanet " + byType.PermanetBonus.ToString());
								foreach (RawBonus rawBonus2 in rawBonus)
								{
									base.Console.Write("...Base:" + rawBonus2.Base.ToString() + "  Multyplier:" + rawBonus2.Multiplier.ToString());
								}
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use " + command + " help");
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"bonus"
			};
		}
	}
}
