using System;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class MaxFervour : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			if (Core.Logic.Penitent != null)
			{
				Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
			}
		}

		public override string GetName()
		{
			return "maxfervour";
		}
	}
}
