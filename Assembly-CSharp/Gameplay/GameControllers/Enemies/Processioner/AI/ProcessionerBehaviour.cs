using System;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Processioner.AI
{
	public class ProcessionerBehaviour : EnemyBehaviour
	{
		public override void OnStart()
		{
			base.OnStart();
			this._processioner = (Processioner)this.Entity;
			this._processioner.OnDeath += this.OnDeath;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this._processioner.ContactDamage.IsTargetOverlapped && !this._processioner.Status.Dead)
			{
				this._contactDamageCooldown += Time.deltaTime;
				if (this._contactDamageCooldown >= 0.1f)
				{
					this._contactDamageCooldown = 0f;
					IDamageable component = this._processioner.Target.GetComponent<IDamageable>();
					if (component != null)
					{
						this._processioner.ContactDamage.EntityContactDamage(component);
					}
				}
			}
			else
			{
				this._contactDamageCooldown = 0f;
			}
		}

		public bool CanMove
		{
			get
			{
				return !this._processioner.MotionChecker.HitsBlock;
			}
		}

		public bool TargetSeen
		{
			get
			{
				return this._processioner.VisionCone.CanSeeTarget(this._processioner.Target.transform, "Penitent", false);
			}
		}

		private void OnDeath()
		{
			this._processioner.OnDeath -= this.OnDeath;
			base.EnableColliders(false);
			if (this._processioner.ChainedAngel == null || !this._processioner.ChainedAngel.StateMachine.enabled)
			{
				return;
			}
			this._processioner.ChainedAngel.StateMachine.enabled = false;
			this._processioner.ChainedAngel.AnimatorInjector.Death();
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		private Processioner _processioner;

		private float _contactDamageCooldown;
	}
}
