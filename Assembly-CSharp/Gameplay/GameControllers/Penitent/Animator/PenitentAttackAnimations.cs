using System;
using System.Collections.Generic;
using DamageEffect;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Player.Dust;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Attack;
using Gameplay.GameControllers.Penitent.Damage;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Animator
{
	public class PenitentAttackAnimations : AttackAnimationsEvents
	{
		private void Awake()
		{
			this.PenitentAttack = base.transform.parent.GetComponentInChildren<PenitentAttack>();
			this._penitent = base.GetComponentInParent<Penitent>();
			this._playerAnimator = base.GetComponent<Animator>();
			this._parryDust = base.GetComponentInChildren<ParryDust>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._currentLevel = Core.Logic.CurrentLevelConfig;
			this._cameraManager = Core.Logic.CameraManager;
			this._spriteEffects = base.GetComponent<MasterShaderEffects>();
			PenitentDamageArea damageArea = this._penitent.DamageArea;
			damageArea.OnDamaged = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(damageArea.OnDamaged, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamaged));
		}

		private Material GetMaterialByHit(Hit h)
		{
			return this.damageMaterials.Find((MaterialsPerDamageElement x) => x.element == h.DamageElement).mat;
		}

		private void OnDamaged(Penitent damaged, Hit hit)
		{
			if (hit.DamageElement == DamageArea.DamageElement.Contact || hit.DamageElement == DamageArea.DamageElement.Normal)
			{
				if (this._spriteEffects != null)
				{
					this._spriteEffects.TriggerColorFlash();
				}
			}
			else
			{
				Material materialByHit = this.GetMaterialByHit(hit);
				if (this._spriteEffects != null)
				{
					this._spriteEffects.DamageEffectBlink(0f, 0.2f, materialByHit);
				}
			}
		}

		public override void WeaponBlowUp(float bladeBlowUp)
		{
			bool isWeaponBlowingUp = (int)Mathf.Clamp01(bladeBlowUp) > 0;
			if (this.PenitentAttack != null)
			{
				this.PenitentAttack.IsWeaponBlowingUp = isWeaponBlowingUp;
			}
		}

		public void DamageByFall(int fallingDamage, float killByFallDamageThreshold)
		{
			if ((float)fallingDamage <= 0f)
			{
				return;
			}
			float num = this._penitent.Stats.Life.Current;
			if ((float)fallingDamage >= num * killByFallDamageThreshold)
			{
				if (!this._penitent.IsSmashed)
				{
					this._penitent.IsSmashed = true;
					this._playerAnimator.Play("Death Fall");
				}
			}
			else
			{
				this._penitent.Damage((float)fallingDamage, string.Empty);
			}
		}

		public void LevelSleepTime(float sleepTime)
		{
			if (this._currentLevel == null)
			{
				return;
			}
			this._currentLevel.sleepTime = sleepTime;
			if (!this._currentLevel.IsSleeping)
			{
				this._currentLevel.SleepTime();
			}
		}

		public void ComboCameraShake()
		{
			if (this._cameraManager != null && this._penitent.AttackArea.IsEnemyHit() && this._cameraManager.ProCamera2DShake != null)
			{
				this._cameraManager.ProCamera2DShake.ShakeUsingPreset("FinalHitCombo");
			}
		}

		public void ChargedAttackCameraShake()
		{
			if (this._cameraManager == null)
			{
				return;
			}
			if (this._penitent.AttackArea.IsEnemyHit() && this._cameraManager.ProCamera2DShake != null)
			{
				this._cameraManager.ProCamera2DShake.ShakeUsingPreset("ChargedAttack");
			}
		}

		public void SetSwordSlash(PenitentSword.AttackType type)
		{
			PenitentSword penitentSword = (PenitentSword)this._penitent.PenitentAttack.CurrentPenitentWeapon;
			PenitentSword.SwordSlash slashAnimation = new PenitentSword.SwordSlash
			{
				Type = type,
				Level = this._penitent.PenitentAttack.CurrentLevel
			};
			PenitentSword.AttackColor attackColor = (slashAnimation.Level <= 1) ? PenitentSword.AttackColor.Default : this._penitent.PenitentAttack.AttackColor;
			slashAnimation.Color = penitentSword.SlashAnimator.GetColorValue(attackColor);
			penitentSword.SlashAnimator.SetSlashAnimation(slashAnimation);
		}

		public void FinishingUpwardCombo()
		{
			this._penitent.PenitentAttack.CurrentWeaponAttack(DamageArea.DamageType.Heavy, true);
		}

		public void FinishingDownwardCombo()
		{
			this._penitent.PenitentAttack.CurrentWeaponAttack(DamageArea.DamageType.Heavy, true);
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			if (this.PenitentAttack == null)
			{
				return;
			}
			this.PenitentAttack.CurrentWeaponAttack(damageType);
		}

		public override void CurrentWeaponRawAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponRawAttack(damageType);
			this.PenitentAttack.CurrentWeaponAttack(damageType, false);
		}

		public void OpenParryWindow()
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.Parry.IsOnParryChance = true;
		}

		public void CloseParryWindow()
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.Parry.IsOnParryChance = false;
		}

		public void TriggerParryEffect()
		{
		}

		public void RaiseParryDust()
		{
			if (this._parryDust == null)
			{
				return;
			}
			this._parryDust.TriggerParryDust();
		}

		public void PrayerUseAttack()
		{
			if (this._penitent == null || !this._penitent.Status.IsGrounded)
			{
				return;
			}
			VerticalAttack componentInChildren = this._penitent.GetComponentInChildren<VerticalAttack>();
			if (componentInChildren == null)
			{
				return;
			}
			componentInChildren.EnableVerticalAttackCollider(true);
			componentInChildren.TriggerVerticalAttack(false);
			PrayerUse componentInChildren2 = this._penitent.GetComponentInChildren<PrayerUse>();
			componentInChildren2.Cast();
		}

		public void InstanceRangeAttackProjectile()
		{
			if (this._penitent == null)
			{
				return;
			}
			RangeAttack componentInChildren = this._penitent.GetComponentInChildren<RangeAttack>();
			if (componentInChildren.Casting)
			{
				componentInChildren.InstanceProjectile();
			}
		}

		public void CanLungeAttack(AttackAnimationsEvents.Activation activation)
		{
			LungeAttack componentInChildren = Core.Logic.Penitent.GetComponentInChildren<LungeAttack>();
			componentInChildren.CanHit = (activation == AttackAnimationsEvents.Activation.True);
		}

		public void FireChargedAttackProjectile()
		{
			this._penitent.ChargedAttack.InstantiateProjectile();
		}

		public void OpenAttackWindow()
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.PenitentAttack.WindowAttackOpen = true;
		}

		public void CloseAttackWindow()
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.PenitentAttack.WindowAttackOpen = false;
		}

		public void CastActiveRiposte()
		{
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.ActiveRiposte.Cast();
		}

		public void PlayRangeAttack()
		{
			this._penitent.Audio.PlayRangeAttack();
		}

		public void PlayBasicAttack1()
		{
			this._penitent.Audio.PlayBasicAttack1();
		}

		public void PlayBasicAttack2()
		{
			this._penitent.Audio.PlayBasicAttack2();
		}

		public void PlayAirAttack1()
		{
			this._penitent.Audio.PlayAirAttack1();
		}

		public void PlayAirAttack2()
		{
			this._penitent.Audio.PlayBasicAttack2();
		}

		public void PlayHeavyAttack()
		{
			this._penitent.Audio.PlayHeavyAttack();
		}

		public void PlayLoadingChargeAttack()
		{
		}

		public void PlayLoadedChargedAttack()
		{
		}

		public void PlayParryHit()
		{
			this._penitent.Audio.PlayParryHit();
		}

		public void PlayReleaseChargeAttack()
		{
			this._penitent.Audio.PlayReleaseChargedAttack();
		}

		public void PlayParry()
		{
			this._penitent.Audio.PlayParryAttack();
		}

		public void StartParry()
		{
			this._penitent.Audio.PlayStartParry();
		}

		public void PlayOverThrow()
		{
			this._penitent.Audio.PlayOverthrow();
		}

		public void PlayDeath()
		{
			this._penitent.Audio.PlayDeath();
		}

		public void PlayDeathSpikes()
		{
			this._penitent.Audio.PlayDeathSpikes();
		}

		public void PlayDeathFall()
		{
			this._penitent.Audio.PlayDeathFall();
		}

		public void PlaySimpleDamage()
		{
			this._penitent.Audio.PlaySimpleDamage();
		}

		public void PlayPushBack()
		{
			this._penitent.Audio.PlayPushBack();
		}

		public void PlayVerticalAttackStart()
		{
			this._penitent.Audio.PlayVerticalAttackStart();
		}

		public void PlayVerticalAttackFalling()
		{
			this._penitent.Audio.PlayVerticalAttackFalling();
		}

		public void PlayVerticalAttackLanding()
		{
			this._penitent.Audio.PlayVerticalAttackLanding();
		}

		public void PlayFinishingComboDown()
		{
			this._penitent.Audio.PlayFinishingComboDown();
		}

		public void PlayHealingExplosion()
		{
			this._penitent.Audio.HealingExplosion();
		}

		public void PlayPrayerUse()
		{
			this._penitent.Audio.ActivatePrayer();
		}

		public void PlayComboHit()
		{
			this._penitent.Audio.PlayComboHit();
		}

		public void PlayComboHitUp()
		{
			this._penitent.Audio.PlayComboHitUp();
		}

		public void PlayComboHitDown()
		{
			this._penitent.Audio.PlayComboHitDown();
		}

		public List<MaterialsPerDamageElement> damageMaterials;

		protected PenitentAttack PenitentAttack;

		private Penitent _penitent;

		private Animator _playerAnimator;

		private LevelInitializer _currentLevel;

		private CameraManager _cameraManager;

		private DamageEffectScript _flash;

		private MasterShaderEffects _spriteEffects;

		private ParryDust _parryDust;
	}
}
