using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class SoftLanding : Ability
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._damageableEntities = new List<IDamageable>();
			this.EnableVerticalAttackCollider(false);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Animator.GetCurrentAnimatorStateInfo(0).IsName("HardLanding") && !base.Casting)
			{
				base.Cast();
			}
			if (!base.Animator.GetCurrentAnimatorStateInfo(0).IsName("HardLanding") && base.Casting)
			{
				base.StopCast();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this.EnableVerticalAttackCollider(true);
			Hit hardLandingHit = this.GetHardLandingHit();
			this.AttackDamageableEntities(hardLandingHit);
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			if (this._attack)
			{
				this._attack = !this._attack;
			}
			this.EnableVerticalAttackCollider(false);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
		}

		private List<IDamageable> GetDamageableEntities()
		{
			GameObject[] array = this.HardLandingAttackArea.OverlappedEntities();
			int num = array.Length;
			byte b = 0;
			while ((int)b < num)
			{
				IDamageable componentInParent = array[(int)b].GetComponentInParent<IDamageable>();
				this._damageableEntities.Add(componentInParent);
				b += 1;
			}
			return this._damageableEntities;
		}

		private void AttackDamageableEntities(Hit hardLandingHit)
		{
			List<IDamageable> damageableEntities = this.GetDamageableEntities();
			int count = damageableEntities.Count;
			if (count <= 0)
			{
				return;
			}
			byte b = 0;
			while ((int)b < count)
			{
				this._damageableEntities[(int)b].Damage(hardLandingHit);
				b += 1;
			}
			this._damageableEntities.Clear();
		}

		private Hit GetHardLandingHit()
		{
			return new Hit
			{
				AttackingEntity = base.EntityOwner.gameObject,
				DamageType = DamageArea.DamageType.Normal,
				DamageAmount = base.EntityOwner.Stats.Strength.Final
			};
		}

		private void EnableVerticalAttackCollider(bool enable = true)
		{
			if (enable)
			{
				if (!this.HardLandingAttackArea.WeaponCollider.enabled)
				{
					this.HardLandingAttackArea.WeaponCollider.enabled = true;
				}
				if (!this.HardLandingAttackArea.enabled)
				{
					this.HardLandingAttackArea.enabled = true;
				}
			}
			else
			{
				if (this.HardLandingAttackArea.WeaponCollider.enabled)
				{
					this.HardLandingAttackArea.WeaponCollider.enabled = false;
				}
				if (this.HardLandingAttackArea.enabled)
				{
					this.HardLandingAttackArea.enabled = false;
				}
			}
		}

		private bool _attack;

		public bool Active;

		public AttackArea HardLandingAttackArea;

		private List<IDamageable> _damageableEntities;
	}
}
