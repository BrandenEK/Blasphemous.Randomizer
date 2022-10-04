using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Attack;
using Gameplay.GameControllers.Penitent.Audio;
using Gameplay.UI;
using Tools.Level;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class Parry : Ability
	{
		public bool SuccessParry { get; set; }

		public bool IsOnParryChance { get; set; }

		public float ParryWindow { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			if (this.Sword == null)
			{
				Debug.LogError("This component needs and Penitent Attack Component");
			}
			Interactable.SInteractionStarted += this.OnSInteractionStarted;
		}

		private void OnDestroy()
		{
			Interactable.SInteractionStarted -= this.OnSInteractionStarted;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			bool flag = this.IsGrounded();
			if (UIController.instance.IsTutorialActive())
			{
				this.StopParry();
			}
			if (this.ParryInput)
			{
				if (!flag || this.IsRunningParryAnim || !this.ReadyToCast() || this.SuccessParry || Core.Input.InputBlocked)
				{
					return;
				}
				this.RaiseParryEvent();
				base.Cast();
			}
			else
			{
				if (!base.Casting || Core.Logic.Penitent.GuardSlide.Casting)
				{
					return;
				}
				this.CheckParryWindow();
				Core.Logic.Penitent.Parry.IsOnParryChance = (base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("ParryStart") || base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("ParryChance"));
				if (base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
				{
					base.StopCast();
				}
			}
			if (!base.EntityOwner.Status.IsGrounded || base.EntityOwner.Status.Dead || base.EntityOwner.Status.IsHurt)
			{
				base.StopCast();
			}
		}

		private void RaiseParryEvent()
		{
			if (this.OnParryCast != null)
			{
				this.OnParryCast();
			}
		}

		private void OnSInteractionStarted(Interactable entity)
		{
			if (this.IsOnParryChance)
			{
				this.IsOnParryChance = false;
			}
		}

		private void CheckParryWindow()
		{
			this.ParryWindow -= Time.deltaTime;
			if (this.ParryWindow > 0f)
			{
				return;
			}
			if (!base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("ParryFailed") && !base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !base.EntityOwner.Status.IsHurt && !this.SuccessParry)
			{
				base.EntityOwner.Animator.Play("ParryFailed");
			}
		}

		public bool CheckParry(Hit hit)
		{
			if (!this.IsHitParryable(hit))
			{
				return false;
			}
			this.SuccessParry = this.Sword.SuccessParryChance(hit);
			this.ParrySuccess(this.SuccessParry);
			if (this.SuccessParry)
			{
				Core.Events.LaunchEvent("PARRY", string.Empty);
				Core.Logic.ScreenFreeze.Freeze(0.1f, this.slowTimeDuration, 0f, this.slowTimeCurve);
			}
			return this.SuccessParry;
		}

		private bool IsHitParryable(Hit hit)
		{
			bool result = true;
			switch (hit.DamageElement)
			{
			case DamageArea.DamageElement.Fire:
			case DamageArea.DamageElement.Toxic:
			case DamageArea.DamageElement.Magic:
			case DamageArea.DamageElement.Lightning:
			case DamageArea.DamageElement.Contact:
				result = false;
				break;
			}
			DamageArea.DamageType damageType = hit.DamageType;
			if (damageType == DamageArea.DamageType.Heavy || damageType == DamageArea.DamageType.Simple)
			{
				result = false;
			}
			return result;
		}

		public void ParrySuccess(bool success)
		{
			Core.Metrics.CustomEvent("PARRY_SUCCESS", string.Empty, -1f);
			if (success)
			{
				base.EntityOwner.Status.Invulnerable = true;
			}
			base.EntityOwner.Animator.SetBool(Parry.Parry1, this.SuccessParry);
			if (this.SuccessParry)
			{
				base.EntityOwner.GetComponentInChildren<PenitentAudio>().PlayParryAttack();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this.StartParry();
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			this.StopParry();
		}

		private void StartParry()
		{
			Core.Metrics.CustomEvent("PARRY_USED", string.Empty, -1f);
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			if (Core.Logic.Penitent.PenitentAttack.IsRunningCombo)
			{
				Core.Logic.Penitent.CancelEffect.PlayCancelEffect();
			}
			base.EntityOwner.Animator.Play(this._startParryAnim);
			this.ParryWindow = base.EntityOwner.Stats.ParryWindow.Final;
			Core.Audio.EventOneShotPanned(this.ParryGuardFx, base.EntityOwner.transform.position);
		}

		private void StopParry()
		{
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			base.EntityOwner.Status.Invulnerable = false;
			this.SuccessParry = false;
			base.EntityOwner.Animator.SetBool(Parry.Parry1, this.SuccessParry);
			Core.Logic.Penitent.Parry.IsOnParryChance = false;
			Core.Logic.Penitent.Audio.StopParryFx();
		}

		private bool IsGrounded()
		{
			if (base.EntityOwner.Status.IsGrounded)
			{
				this._timeGrounded += Time.deltaTime;
			}
			else
			{
				this._timeGrounded = 0f;
			}
			return this._timeGrounded > 0.1f;
		}

		private bool ReadyToCast()
		{
			bool flag = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("HardLanding");
			bool flag2 = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Charged Attack");
			bool isHurt = base.EntityOwner.Status.IsHurt;
			return !flag && !isHurt && !flag2;
		}

		private bool ParryInput
		{
			get
			{
				return base.Rewired.GetButtonDown(38);
			}
		}

		public bool IsRunningParryAnim
		{
			get
			{
				bool flag = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("ParryStart");
				bool flag2 = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("ParryChance");
				bool flag3 = base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("ParrySuccess");
				return flag || flag2 || flag3;
			}
		}

		public Core.SimpleEvent OnParryCast;

		public PenitentSword Sword;

		private float _timeGrounded;

		private const int RightButton = 1;

		public float slowTimeDuration = 0.3f;

		public AnimationCurve slowTimeCurve;

		[EventRef]
		public string ParryGuardFx;

		private readonly int _startParryAnim = Animator.StringToHash("ParryStart");

		private static readonly int Parry1 = Animator.StringToHash("PARRY");
	}
}
