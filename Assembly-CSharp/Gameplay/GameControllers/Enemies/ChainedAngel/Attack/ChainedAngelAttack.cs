using System;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Enemies.ChainedAngel.Attack
{
	public class ChainedAngelAttack : EnemyAttack
	{
		private AttackArea AttackArea { get; set; }

		private ChainedAngel ChainedAngel { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<ChainedAngelWeapon>();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.ChainedAngel = (ChainedAngel)base.EntityOwner;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnStay += this.OnStayAttackArea;
			this.AttackArea.OnExit += this.OnExitAttackArea;
			this._weaponHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = DamageArea.DamageType.Normal,
				DamageElement = DamageArea.DamageElement.Normal,
				forceGuardslide = true,
				DontSpawnBlood = false,
				Force = this.Force,
				HitSoundId = this.HitSound
			};
		}

		private void OnStayAttackArea(object sender, Collider2DParam e)
		{
			if (this._targetAttacked || !this.ChainedAngel.BodyChainMaster.IsAttacking)
			{
				return;
			}
			base.CurrentEnemyWeapon.Attack(this._weaponHit);
			this._targetAttacked = true;
		}

		private void OnExitAttackArea(object sender, Collider2DParam e)
		{
			this._targetAttacked = false;
		}

		private void OnDestroy()
		{
			this.AttackArea.OnStay -= this.OnStayAttackArea;
			this.AttackArea.OnExit -= this.OnExitAttackArea;
		}

		private bool _targetAttacked;

		private Hit _weaponHit;
	}
}
