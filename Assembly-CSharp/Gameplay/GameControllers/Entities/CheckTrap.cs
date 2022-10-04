using System;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(Collider2D))]
	public class CheckTrap : Trait
	{
		public bool AvoidTrapDamage { get; set; }

		public bool DeathBySpike { get; private set; }

		public bool DeathByFall { get; private set; }

		private BoxCollider2D FloorCollider { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.DeathBySpike = false;
			this.DeathByFall = false;
			this.FloorCollider = base.GetComponent<BoxCollider2D>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.CompareTag("SpikeTrap"))
			{
				if (!this.OverlapTrapCollider(other))
				{
					return;
				}
				this.DeathBySpike = true;
				this.SpikeTrapDamage();
				Core.Events.LaunchEvent("PENITENT_KILLED", "SPIKES");
			}
			else if (other.gameObject.CompareTag("AbyssTrap") && !this.AvoidTrapDamage)
			{
				this.DeathByAbyssTrap();
			}
			else if (other.gameObject.CompareTag("UnnavoidableTrap"))
			{
				this.DeathByAbyssTrap();
			}
			else if (other.gameObject.CompareTag("HWTrap"))
			{
				this.DeathByHWTrap();
			}
		}

		private void DeathByAbyssTrap()
		{
			this.AbyssTrapDamage();
			this.DeathByFall = true;
			Core.Events.LaunchEvent("PENITENT_KILLED", "ABYSS");
		}

		private void DeathByHWTrap()
		{
			Hit hit = new Hit
			{
				DamageAmount = Core.Logic.Penitent.Stats.Life.Current,
				AttackingEntity = Core.Logic.Penitent.gameObject,
				DamageType = DamageArea.DamageType.Heavy
			};
			Core.Logic.Penitent.DamageArea.TakeDamage(hit, true);
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (other.gameObject.CompareTag("SpikeTrap"))
			{
				if (!this.OverlapTrapCollider(other) || this.DeathBySpike)
				{
					return;
				}
				this.DeathBySpike = true;
				this.SpikeTrapDamage();
				Core.Events.LaunchEvent("PENITENT_KILLED", "SPIKES");
			}
			else if (other.gameObject.CompareTag("HWTrap"))
			{
				this.DeathByHWTrap();
			}
		}

		private void SpikeTrapDamage()
		{
			if (base.EntityOwner == null)
			{
				return;
			}
			base.EntityOwner.IsImpaled = true;
			if (base.EntityOwner.Stats.Life.Current > 0f)
			{
				base.EntityOwner.Stats.Life.Current = 0f;
			}
			base.EntityOwner.KillInstanteneously();
			PlatformCharacterController componentInChildren = base.EntityOwner.GetComponentInChildren<PlatformCharacterController>();
			if (componentInChildren)
			{
				componentInChildren.enabled = false;
			}
		}

		private bool OverlapTrapCollider(Collider2D trapCollider)
		{
			return this.FloorCollider.bounds.min.y + 0.15f <= trapCollider.bounds.max.y && !this.IsOwnerThrown;
		}

		private bool IsOwnerThrown
		{
			get
			{
				return Core.Logic.Penitent.ThrowBack.IsThrown;
			}
		}

		private void AbyssTrapDamage()
		{
			if (base.EntityOwner == null)
			{
				return;
			}
			if (base.EntityOwner.Stats.Life.Current > 0f)
			{
				base.EntityOwner.Stats.Life.Current = 0f;
			}
			base.EntityOwner.KillInstanteneously();
			PlatformCharacterController componentInChildren = base.EntityOwner.GetComponentInChildren<PlatformCharacterController>();
			if (componentInChildren)
			{
				componentInChildren.enabled = false;
			}
			if (base.EntityOwner.GetType() == typeof(Penitent))
			{
				Core.Logic.CameraManager.ProCamera2D.FollowVertical = false;
			}
		}

		public const string PenitentKilled = "PENITENT_KILLED";

		public const string SpikesTrap = "SPIKES";

		public const string AbyssTrap = "ABYSS";

		private const float ColliderOverlapVOffset = 0.15f;
	}
}
