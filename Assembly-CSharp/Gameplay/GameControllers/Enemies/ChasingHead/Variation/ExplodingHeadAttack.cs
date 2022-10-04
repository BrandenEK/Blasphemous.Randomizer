using System;
using DG.Tweening;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.ChasingHead.AI;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChasingHead.Variation
{
	public class ExplodingHeadAttack : EnemyAttack
	{
		public AttackArea AttackArea { get; set; }

		public bool EntityAttacked { get; set; }

		public ChasingHead _chasingHead { get; private set; }

		private ChasingHeadBehaviour ChasingHeadBehaviour { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.EntityOwner.GetComponentInChildren<Weapon>();
			this.AttackArea = base.EntityOwner.GetComponentInChildren<AttackArea>();
			this.ChasingHeadBehaviour = base.EntityOwner.GetComponentInChildren<ChasingHeadBehaviour>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._enemyLayer = LayerMask.NameToLayer("Enemy");
			float final = base.EntityOwner.Stats.Strength.Final;
			GameObject gameObject = base.EntityOwner.gameObject;
			this._chasingHeadNormalHit = new Hit
			{
				AttackingEntity = gameObject,
				DamageType = DamageArea.DamageType.Normal,
				DamageAmount = this.ContactDamageAmount,
				HitSoundId = this.HitSound,
				Force = this.Force
			};
			this._chasingHeadHeavyHit = new Hit
			{
				AttackingEntity = gameObject,
				DamageType = DamageArea.DamageType.Heavy,
				DamageAmount = final,
				HitSoundId = this.HitSound,
				Force = this.Force
			};
			this._chasingHead = (ChasingHead)base.EntityOwner;
			ChasingHeadBehaviour chasingHeadBehaviour = this.ChasingHeadBehaviour;
			chasingHeadBehaviour.OnHurtDisplacement = (Core.SimpleEvent)Delegate.Combine(chasingHeadBehaviour.OnHurtDisplacement, new Core.SimpleEvent(this.OnHurtDisplacement));
			this.AttackArea.OnStay += this.AttackAreaOnStay;
		}

		private void OnHurtDisplacement()
		{
			DOTween.Kill(this._chasingHead.transform, false);
		}

		private void AttackAreaOnStay(object sender, Collider2DParam e)
		{
			if (!e.Collider2DArg.CompareTag("Penitent"))
			{
				return;
			}
			if (this.EntityAttacked)
			{
				return;
			}
			this.EntityAttacked = true;
			this.ContactAttack();
			this._chasingHead.Behaviour.enabled = false;
			base.EntityOwner.Animator.Play(this.DeathAnim);
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			if (base.CurrentEnemyWeapon == null)
			{
				return;
			}
			base.CurrentEnemyWeapon.Attack(this._chasingHeadHeavyHit);
		}

		private void ContactAttack()
		{
			if (base.CurrentEnemyWeapon == null)
			{
				return;
			}
			AttackArea attackArea = this.AttackArea;
			attackArea.enemyLayerMask ^= 1 << this._enemyLayer;
			base.CurrentEnemyWeapon.Attack(this._chasingHeadNormalHit);
		}

		public void RangeAttack()
		{
			this.AttackArea.enemyLayerMask = (this.AttackArea.enemyLayerMask | 1 << this._enemyLayer);
			this.SetDamageAreaExplosionSize();
			this.CurrentWeaponAttack();
		}

		private void SetDamageAreaExplosionSize()
		{
			((BoxCollider2D)this.AttackArea.WeaponCollider).size = new Vector2(4f, 4f);
		}

		private void OnDestroy()
		{
			if (this.AttackArea)
			{
				this.AttackArea.OnStay -= this.AttackAreaOnStay;
			}
			if (this.ChasingHeadBehaviour)
			{
				ChasingHeadBehaviour chasingHeadBehaviour = this.ChasingHeadBehaviour;
				chasingHeadBehaviour.OnHurtDisplacement = (Core.SimpleEvent)Delegate.Remove(chasingHeadBehaviour.OnHurtDisplacement, new Core.SimpleEvent(this.OnHurtDisplacement));
			}
		}

		public readonly int DeathAnim = Animator.StringToHash("Death");

		private Hit _chasingHeadNormalHit;

		private Hit _chasingHeadHeavyHit;

		private int _enemyLayer;
	}
}
