using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Runner.Attack
{
	public class RunnerAttack : EnemyAttack
	{
		public ContactDamage ContactDamage { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.ContactDamage = base.EntityOwner.GetComponentInChildren<ContactDamage>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			ContactDamage contactDamage = this.ContactDamage;
			contactDamage.OnContactDamage = (Core.GenericEvent)Delegate.Combine(contactDamage.OnContactDamage, new Core.GenericEvent(this.OnContactDamage));
			base.EntityOwner.OnDeath += this.OnDeath;
			this.onSuscribeEvents = true;
		}

		private void OnContactDamage(UnityEngine.Object param)
		{
			this.damageableTarget = (IDamageable)param;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!base.EntityOwner || base.EntityOwner.Dead)
			{
				return;
			}
			if (this.ContactDamage.IsTargetOverlapped && this.damageableTarget != null)
			{
				this.contactDamageCoolDown -= Time.deltaTime;
				if (this.contactDamageCoolDown > 0f)
				{
					return;
				}
				this.contactDamageCoolDown = 1f;
				this.ContactAttack(this.damageableTarget);
			}
			else
			{
				this.contactDamageCoolDown = 1f;
			}
		}

		private void OnDeath()
		{
			this.UnsubscribeEvents();
		}

		private void OnDestroy()
		{
			this.UnsubscribeEvents();
		}

		private void UnsubscribeEvents()
		{
			if (!this.onSuscribeEvents)
			{
				return;
			}
			this.onSuscribeEvents = false;
			if (this.ContactDamage)
			{
				ContactDamage contactDamage = this.ContactDamage;
				contactDamage.OnContactDamage = (Core.GenericEvent)Delegate.Remove(contactDamage.OnContactDamage, new Core.GenericEvent(this.OnContactDamage));
			}
			if (base.EntityOwner)
			{
				base.EntityOwner.OnDeath -= this.OnDeath;
			}
		}

		private float contactDamageCoolDown = 1f;

		private IDamageable damageableTarget;

		private bool onSuscribeEvents;
	}
}
