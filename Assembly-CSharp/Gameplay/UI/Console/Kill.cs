using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class Kill : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			if (parameters.Length < 1)
			{
				base.Console.Write("Invalid parameters. Available kill commands are:");
				base.Console.Write("kill player: Kill player");
				base.Console.Write("kill player prieudieu: Kill player and force to respawn in prieudieu");
				base.Console.Write("kill all: Kill all entities");
				base.Console.Write("kill enemies: Kill all entities");
				base.Console.Write("kill NAME: Kill entity with name NAME");
			}
			else if (parameters[0] == "player")
			{
				bool forceInPrieudieu = parameters.Length > 1 && parameters[1] == "prieudieu";
				this.KillPlayer(forceInPrieudieu);
			}
			else if (parameters[0] == "all")
			{
				this.KillAllEntities();
			}
			else if (parameters[0] == "enemies")
			{
				this.KillAllEnemies();
			}
			else
			{
				this.KillSpecific(parameters[0]);
			}
		}

		private void KillPlayer(bool forceInPrieudieu)
		{
			if (forceInPrieudieu && Core.SpawnManager.AutomaticRespawn)
			{
				Core.SpawnManager.IgnoreNextAutomaticRespawn = true;
			}
			base.Penitent.KillInstanteneously();
		}

		private void KillAllEntities()
		{
			Entity[] entities = UnityEngine.Object.FindObjectsOfType<Entity>();
			this.KillAll(entities);
			base.Console.Write("Killing everybody on the scene.");
		}

		private void KillAllEnemies()
		{
			Enemy[] entities = UnityEngine.Object.FindObjectsOfType<Enemy>();
			this.KillAll(entities);
			base.Console.Write("Killing every enemy on the scene.");
		}

		private void KillAll(Entity[] entities)
		{
			foreach (Entity entity in entities)
			{
				IDamageable damageable = entity as IDamageable;
				Hit hit = new Hit
				{
					DamageAmount = float.MaxValue
				};
				if (damageable != null)
				{
					damageable.Damage(hit);
				}
				if (!entity.Status.Dead)
				{
					entity.Damage(float.MaxValue, string.Empty);
				}
				if (!entity.Status.Dead)
				{
					entity.KillInstanteneously();
				}
			}
		}

		private void KillSpecific(string name)
		{
			try
			{
				GameObject gameObject = GameObject.Find(name);
				Entity component = gameObject.GetComponent<Entity>();
				component.KillInstanteneously();
				base.Console.Write("Killing entity: " + component.name);
			}
			catch (NullReferenceException ex)
			{
				base.Console.Write(ex.Message);
			}
		}

		public override string GetName()
		{
			return "kill";
		}
	}
}
