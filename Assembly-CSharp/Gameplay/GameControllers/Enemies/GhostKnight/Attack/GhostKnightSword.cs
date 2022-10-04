using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Enemies.GhostKnight.AI;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent;

namespace Gameplay.GameControllers.Enemies.GhostKnight.Attack
{
	public class GhostKnightSword : Weapon
	{
		public GhostKnightBehaviour Behaviour { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Behaviour = this.WeaponOwner.GetComponentInChildren<GhostKnightBehaviour>();
		}

		public override void Attack(Hit weapondHit)
		{
			this._damageables = base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public bool TargetIsOnParryChance()
		{
			bool result = false;
			for (int i = 0; i < this._damageables.Count; i++)
			{
				Penitent penitent = (Penitent)this._damageables[i];
				if (penitent.Parry.IsOnParryChance)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private List<IDamageable> _damageables;
	}
}
