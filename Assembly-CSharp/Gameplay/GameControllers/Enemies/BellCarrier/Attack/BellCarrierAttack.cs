using System;
using System.Collections;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellCarrier.Attack
{
	public class BellCarrierAttack : EnemyAttack
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._bellCarrier = base.GetComponentInParent<BellCarrier>();
			base.CurrentEnemyWeapon = base.GetComponent<Weapon>();
			this._attackAreasEnabled = true;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.BellCarrierAttackArea.OnStay += this.BellCarrierAttackAreaOnStay;
			this.BellCarrierAttackArea.OnExit += this.BellCarrierAttackAreaOnExit;
			this._bellCarrier.OnDeath += this.BellCarrierOnEntityDie;
		}

		private void BellCarrierAttackAreaOnStay(object sender, Collider2DParam collider2DParam)
		{
			if (!this._bellCarrier.AnimatorInyector.Animator.GetCurrentAnimatorStateInfo(0).IsName("Running") || this._attacked)
			{
				return;
			}
			this._attacked = true;
			Penitent componentInParent = collider2DParam.Collider2DArg.gameObject.GetComponentInParent<Penitent>();
			Hit attackHit = this.GetAttackHit();
			if (componentInParent == null)
			{
				return;
			}
			if (componentInParent.Status.Unattacable)
			{
				componentInParent.DamageArea.TakeDamage(attackHit, true);
			}
			else
			{
				this.CurrentWeaponAttack(attackHit.DamageType);
			}
		}

		private void BellCarrierAttackAreaOnExit(object sender, Collider2DParam collider2DParam)
		{
			this._attacked = false;
		}

		private void BellCarrierOnEntityDie()
		{
			if (this.BellCarrierAttackArea.WeaponCollider.enabled)
			{
				this.BellCarrierAttackArea.WeaponCollider.enabled = false;
			}
			if (this._bellCarrier.EntityDamageArea.DamageAreaCollider.enabled)
			{
				this._bellCarrier.EntityDamageArea.DamageAreaCollider.enabled = false;
			}
		}

		private Hit GetAttackHit()
		{
			return new Hit
			{
				AttackingEntity = this._bellCarrier.gameObject,
				DamageType = DamageArea.DamageType.Heavy,
				DamageAmount = this._bellCarrier.Stats.Strength.Final,
				Force = this.Force,
				HitSoundId = this.HitSound
			};
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			bool isChasing = this._bellCarrier.EnemyBehaviour.IsChasing;
			this.EnableAttackAreas(isChasing);
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			if (base.CurrentEnemyWeapon == null)
			{
				return;
			}
			if (this._bellCarrier.EntityDamageArea.DamageAreaCollider.enabled)
			{
				Hit attackHit = this.GetAttackHit();
				base.CurrentEnemyWeapon.Attack(attackHit);
				this.DisableDamageArea();
			}
		}

		private void DisableDamageArea()
		{
			if (this._bellCarrier.EntityDamageArea.DamageAreaCollider.enabled)
			{
				base.StartCoroutine(this.DisableDamageAreaCoroutine());
			}
		}

		private IEnumerator DisableDamageAreaCoroutine()
		{
			Collider2D damageCollider = this._bellCarrier.EntityDamageArea.DamageAreaCollider;
			if (damageCollider.enabled)
			{
				damageCollider.enabled = false;
			}
			yield return new WaitForSeconds(0.5f);
			if (!damageCollider.enabled)
			{
				damageCollider.enabled = true;
			}
			yield break;
		}

		private void EnableAttackAreas(bool attackAreaEnabled = true)
		{
			if (attackAreaEnabled)
			{
				if (this._attackAreasEnabled)
				{
					return;
				}
				this._attackAreasEnabled = true;
				this.SetAttackAreaStatus(true);
			}
			else
			{
				if (!this._attackAreasEnabled)
				{
					return;
				}
				this._attackAreasEnabled = false;
				this.SetAttackAreaStatus(false);
			}
		}

		private void SetAttackAreaStatus(bool attackAreaEnabled)
		{
			this.BellCarrierAttackArea.WeaponCollider.enabled = attackAreaEnabled;
		}

		private void OnDestroy()
		{
			this._bellCarrier.OnDeath -= this.BellCarrierOnEntityDie;
		}

		private bool _attackAreasEnabled;

		private bool _attacked;

		private BellCarrier _bellCarrier;

		[SerializeField]
		protected AttackArea BellCarrierAttackArea;
	}
}
