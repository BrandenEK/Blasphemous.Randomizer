using System;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class FervourRefill : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			FervourRefill.enabled = !FervourRefill.enabled;
			base.Console.Write("Fervour Refill: " + ((!FervourRefill.enabled) ? "DISABLED" : "ENABLED"));
		}

		public override void Update()
		{
			if (Core.Logic.Penitent != null && FervourRefill.enabled)
			{
				Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
			}
		}

		public override string GetName()
		{
			return "fervourrefill";
		}

		private static bool enabled;
	}
}
