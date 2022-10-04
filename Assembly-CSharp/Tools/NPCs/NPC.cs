using System;
using System.Diagnostics;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.NPCs.Damage;
using UnityEngine;

namespace Tools.NPCs
{
	[SelectionBase]
	public class NPC : Entity, IDamageable
	{
		public NpcDamage DamageArea { get; private set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.ObjectEvent SCreated;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent SAttacked;

		protected override void OnAwake()
		{
			base.OnAwake();
			this.DamageArea = base.GetComponentInChildren<NpcDamage>();
			if (NPC.SCreated != null)
			{
				NPC.SCreated(base.gameObject);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
			this.Status.IsGrounded = true;
			if (!this.CanTakeDamage && this.DamageArea && this.DamageArea.gameObject != null)
			{
				this.DamageArea.gameObject.layer = LayerMask.NameToLayer("Default");
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsVisibleOnCamera = this.IsVisible();
		}

		public bool IsVisible()
		{
			return Entity.IsVisibleFrom(base.SpriteRenderer, Camera.main);
		}

		public void Damage(Hit hit)
		{
			if (!this.DamageArea || !this.CanTakeDamage)
			{
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (NPC.SAttacked != null)
			{
				NPC.SAttacked();
			}
			if (this.Status.Dead)
			{
				this.Kill();
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public bool CanTakeDamage;
	}
}
