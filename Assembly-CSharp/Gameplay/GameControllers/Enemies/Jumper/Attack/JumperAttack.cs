using System;
using System.Linq;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Jumper.Attack
{
	public class JumperAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; set; }

		public bool TargetInAttackArea { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.AttackArea.OnEnter += this.OnEnterAttackArea;
			this.AttackArea.OnExit += this.OnExitAttackArea;
			this._jumperHit = new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageAmount = base.EntityOwner.Stats.Strength.Final,
				DamageType = DamageArea.DamageType.Normal,
				HitSoundId = this.HitSound,
				Unnavoidable = false
			};
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			GameObject[] source = this.AttackArea.OverlappedEntities();
			GameObject gameObject = source.FirstOrDefault((GameObject overlappedEntity) => overlappedEntity.gameObject.CompareTag("Penitent"));
			if (gameObject != null)
			{
				gameObject.GetComponentInParent<IDamageable>().Damage(this._jumperHit);
			}
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			if (base.EntityOwner.Status.IsGrounded)
			{
				return;
			}
			this.TargetInAttackArea = true;
			this.CurrentWeaponAttack();
		}

		private void OnExitAttackArea(object sender, Collider2DParam e)
		{
			this.TargetInAttackArea = false;
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

		private Hit _jumperHit;
	}
}
