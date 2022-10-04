using System;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Attack
{
	public class CloisteredGemProjectileAttack : BossStraightProjectileAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (!this.useStrengthDamageBonus)
			{
				return;
			}
			this.penitent = Core.Logic.Penitent;
			this.SetProjectileDamage();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (CloisteredGemProjectileAttack.Abs(this.penitent.Stats.Strength.Final - this.currentPenitentStregth) > Mathf.Epsilon)
			{
				this.SetProjectileDamage();
			}
		}

		private void SetProjectileDamage()
		{
			this.currentPenitentStregth = this.penitent.Stats.Strength.Final;
			int projectileWeaponDamage = (int)((float)this.ProjectileDamageAmount + this.currentPenitentStregth);
			base.SetProjectileWeaponDamage(projectileWeaponDamage);
		}

		private static float Abs(float num)
		{
			return (num >= 0f) ? num : (-num);
		}

		public bool useStrengthDamageBonus;

		public float currentPenitentStregth;

		private Penitent penitent;
	}
}
