using System;
using System.Linq;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ReekLeader.Attack
{
	public class ReekLeaderAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; private set; }

		public bool IsTargetReachable { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.AttackArea.OnEnter += this.OnEnterAttackArea;
			this.AttackArea.OnExit += this.OnExitAttackArea;
			this._reekHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = this.DamageType,
				Force = 0f,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			this.IsTargetReachable = true;
		}

		private void OnExitAttackArea(object sender, Collider2DParam e)
		{
			this.IsTargetReachable = false;
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			if (this.AttackArea == null || !this.IsTargetReachable)
			{
				return;
			}
			GameObject[] source = this.AttackArea.OverlappedEntities();
			GameObject gameObject = source.FirstOrDefault((GameObject target) => target.CompareTag("Penitent"));
			if (gameObject != null)
			{
				gameObject.GetComponentInParent<IDamageable>().Damage(this._reekHit);
			}
		}

		private void OnDestroy()
		{
			if (!this.AttackArea)
			{
				return;
			}
			this.AttackArea.OnEnter -= this.OnEnterAttackArea;
			this.AttackArea.OnExit -= this.OnExitAttackArea;
		}

		private Hit _reekHit;
	}
}
