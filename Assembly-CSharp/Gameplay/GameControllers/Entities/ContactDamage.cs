using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(Collider2D))]
	public class ContactDamage : Trait
	{
		public bool IsTargetOverlapped { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.ContactDamageCollider = base.GetComponent<Collider2D>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (base.EntityOwner)
			{
				this.EntityAttack = base.EntityOwner.EntityAttack;
			}
			this.CenterContactDamageCollider();
		}

		public void EntityContactDamage(IDamageable damageable)
		{
			if (this.EntityAttack == null)
			{
				return;
			}
			this.EntityAttack.ContactAttack(damageable);
			if (this.OnContactDamage != null)
			{
				this.OnContactDamage(damageable as Object);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.DamageableLayers.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.IsTargetOverlapped = true;
			Enemy enemy = base.EntityOwner as Enemy;
			if (enemy == null)
			{
				return;
			}
			IDamageable componentInParent = other.GetComponentInParent<IDamageable>();
			if (!enemy.Status.Dead && !enemy.IsStunt && enemy.DamageByContact)
			{
				this.EntityContactDamage(componentInParent);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.DamageableLayers.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.IsTargetOverlapped = false;
		}

		protected void CenterContactDamageCollider()
		{
			Vector2 offset = this.ContactDamageCollider.offset;
			offset.x = 0f;
			this.ContactDamageCollider.offset = offset;
		}

		public Core.GenericEvent OnContactDamage;

		public LayerMask DamageableLayers;

		protected Collider2D ContactDamageCollider;

		protected Attack EntityAttack;
	}
}
