using System;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Enemies.ChimeRinger.Attack
{
	public class ChimeRingerAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
		}

		private Hit _weaponHit;
	}
}
