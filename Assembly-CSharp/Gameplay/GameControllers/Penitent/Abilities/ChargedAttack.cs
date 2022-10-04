using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Attack;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class ChargedAttack : Ability
	{
		public bool IsChargingAttack
		{
			get
			{
				return this._isChargingAttack;
			}
			set
			{
				this._isChargingAttack = value;
			}
		}

		public RootMotionDriver RootMotionDriver { get; private set; }

		public bool IsAvailableSkilledAbility
		{
			get
			{
				return base.CanExecuteSkilledAbility() && base.HasEnoughFervour;
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._penitent = (Penitent)base.EntityOwner;
			this._attackArea = this._penitent.AttackArea;
			this.RootMotionDriver = this._penitent.GetComponentInChildren<RootMotionDriver>();
			this._defaultEnemyScopeDetection = this._attackArea.entityScopeDetection;
			PoolManager.Instance.CreatePool(this.ChargedAttackProjectile, 1);
		}

		protected override void OnUpdate()
		{
			if (!base.Casting)
			{
				return;
			}
			this._currentChargingTime -= Time.deltaTime;
			if (this._currentChargingTime <= 0f)
			{
				base.EntityOwner.Animator.SetBool("CHARGE_ATTACK_TIER", true);
			}
			if (!this._penitent.PlatformCharacterInput.IsAttackButtonHold)
			{
				base.StopCast();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this._isChargingAttack = true;
			this.SetChargingTimeByTier();
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			this._isChargingAttack = false;
			base.EntityOwner.Animator.SetBool("CHARGE_ATTACK_TIER", false);
		}

		public void ResizeAttackArea(bool resize = true)
		{
			if (resize && !this._isAttackAreaResized)
			{
				this._isAttackAreaResized = true;
				this._attackArea.entityScopeDetection = this.chargedAttackScopeDetection;
				this._attackArea.SetSize(PenitentSword.MaxSizeAttackCollider);
			}
			else if (!resize && this._isAttackAreaResized)
			{
				this._isAttackAreaResized = false;
				this._attackArea.entityScopeDetection = this._defaultEnemyScopeDetection;
				this._attackArea.SetSize(PenitentSword.MinSizeAttackCollider);
			}
		}

		private void SetChargingTimeByTier()
		{
			UnlockableSkill lastUnlockedSkill = base.GetLastUnlockedSkill();
			if (lastUnlockedSkill != null)
			{
				string id = lastUnlockedSkill.id;
				this._currentChargingTime = ((!id.Equals("CHARGED_1")) ? this.BaseChargingTimeTier2 : this.BaseChargingTimeTier1);
			}
		}

		public void InstantiateProjectile()
		{
			if (!base.CanExecuteSkilledAbility())
			{
				return;
			}
			if (!base.GetLastUnlockedSkill().id.Equals("CHARGED_3"))
			{
				return;
			}
			if (this.ChargedAttackProjectile == null)
			{
				return;
			}
			Vector3 position = (this._penitent.Status.Orientation != EntityOrientation.Right) ? this.RootMotionDriver.ReversePosition : this.RootMotionDriver.transform.position;
			PoolManager.Instance.ReuseObject(this.ChargedAttackProjectile, position, Quaternion.identity, false, 1);
			this.PlayChargedAttackProjectileFx();
		}

		private void PlayChargedAttackProjectileFx()
		{
			if (string.IsNullOrEmpty(this.ChargedAttackProjectileFx))
			{
				return;
			}
			Core.Audio.PlaySfx(this.ChargedAttackProjectileFx, 0f);
		}

		public float chargedAttackAreaWidth;

		public float chargedAttackScopeDetection;

		public GameObject ChargedAttackProjectile;

		private float _defaultEnemyScopeDetection;

		private bool _isAttackAreaResized;

		private Penitent _penitent;

		private AttackArea _attackArea;

		public float BaseChargingTimeTier1 = 1.5f;

		public float BaseChargingTimeTier2 = 0.75f;

		private float _currentChargingTime;

		private bool _isChargingAttack;

		[EventRef]
		public string ChargedAttackProjectileFx;
	}
}
