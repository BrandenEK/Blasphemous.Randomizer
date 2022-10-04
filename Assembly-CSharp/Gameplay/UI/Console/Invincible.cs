using System;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class Invincible : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			Invincible.enabled = !Invincible.enabled;
			base.Console.Write("Player invulnerability: " + ((!Invincible.enabled) ? "DISABLED" : "ENABLED"));
			Core.SpawnManager.AutomaticRespawn = Invincible.enabled;
		}

		public override void Update()
		{
			if (Core.Logic.Penitent != null && Invincible.enabled)
			{
				Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			}
		}

		public override string GetName()
		{
			return "invincible";
		}

		private static bool enabled;
	}
}
