using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Audio
{
	public class PenitentAudio : EntityAudio
	{
		private void OnDestroy()
		{
			if (this.FloorSensorEmitter != null)
			{
				this.FloorSensorEmitter.OnEnter -= this.FloorSensorEmitterOnEnter;
				this.FloorSensorEmitter.OnExit -= this.FloorSensorListenerOnExit;
				this.FloorSensorEmitter.OnStay -= this.FloorSensorEmitterOnStay;
			}
			if (this.WeaponSensorEmitter != null)
			{
				this.WeaponSensorEmitter.OnStay -= this.WeaponSensorEmitterOnStay;
				this.WeaponSensorEmitter.OnExit -= this.WeaponSensorEmitterOnExit;
			}
			base.ReleaseAudioEvents();
		}

		protected override void OnWake()
		{
			base.OnWake();
			this.EventInstances = new List<EventInstance>();
			if (this.FloorCollider != null)
			{
				this.FloorSensorEmitter = this.FloorCollider.GetComponent<ICollisionEmitter>();
			}
			if (this.WeaponCollider != null)
			{
				this.WeaponSensorEmitter = this.WeaponCollider.GetComponent<ICollisionEmitter>();
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			try
			{
				this.FloorSensorEmitter.OnEnter += this.FloorSensorEmitterOnEnter;
				this.FloorSensorEmitter.OnExit += this.FloorSensorListenerOnExit;
				this.FloorSensorEmitter.OnStay += this.FloorSensorEmitterOnStay;
				this.WeaponSensorEmitter.OnStay += this.WeaponSensorEmitterOnStay;
				this.WeaponSensorEmitter.OnExit += this.WeaponSensorEmitterOnExit;
			}
			catch (NullReferenceException ex)
			{
				if (!this.InitilizationError)
				{
					this.InitilizationError = true;
				}
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private void FloorSensorEmitterOnEnter(object sender, Collider2DParam collider2DParam)
		{
			if ((this.FloorLayerMask.value & 1 << collider2DParam.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
			this.SetParameterValuesByFloor(collider2DParam.Collider2DArg);
		}

		private void FloorSensorListenerOnExit(object sender, Collider2DParam collider2DParam)
		{
			if ((this.FloorLayerMask.value & 1 << collider2DParam.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
		}

		private void FloorSensorEmitterOnStay(object sender, Collider2DParam collider2DParam)
		{
			if ((this.FloorLayerMask.value & 1 << collider2DParam.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
			this.SetParameterValuesByFloor(collider2DParam.Collider2DArg);
		}

		private void WeaponSensorEmitterOnStay(object sender, Collider2DParam collider2DParam)
		{
			if ((this.WeaponLayerMask.value & 1 << collider2DParam.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
			this.SetParametersValuesByEnemy(collider2DParam.Collider2DArg);
		}

		private void WeaponSensorEmitterOnExit(object sender, Collider2DParam collider2DParam)
		{
			if ((this.WeaponLayerMask.value & 1 << collider2DParam.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
		}

		public void SetParametersValuesByWall(Collider2D material)
		{
			if (material.CompareTag("Material:Wood"))
			{
				this.WallWoodValue = 1f;
				this.WallStoneValue = 0f;
			}
			else if (material.CompareTag("Material:Stone"))
			{
				this.WallWoodValue = 0f;
				this.WallStoneValue = 1f;
			}
			else
			{
				this.WallWoodValue = 0f;
				this.WallStoneValue = 1f;
			}
		}

		private void SetParametersValuesByEnemy(Collider2D material)
		{
			if (material.CompareTag("Material:Flesh"))
			{
				this.FleshValue = 1f;
			}
		}

		private void SetParameterValuesByFloor(Collider2D material)
		{
			string tag = material.tag;
			switch (tag)
			{
			case "Material:Dirt":
				this.DirtValue = 1f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Stone":
				this.DirtValue = 0f;
				this.StoneValue = 1f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Snow":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 1f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Wood":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 1f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Marble":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 1f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Water":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 1f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Flesh":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 1f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Metal":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 1f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Mud":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 1f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Secret":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 1f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Grass":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 1f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Demake":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 1f;
				this.PalioValue = 0f;
				this.SnakeValue = 0f;
				return;
			case "Material:Palio":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 1f;
				this.SnakeValue = 0f;
				return;
			case "Material:Snake":
				this.DirtValue = 0f;
				this.StoneValue = 0f;
				this.SnowValue = 0f;
				this.WoodValue = 0f;
				this.MarbleValue = 0f;
				this.WaterValue = 0f;
				this.FleshValue = 0f;
				this.MetalValue = 0f;
				this.MudValue = 0f;
				this.SecretValue = 0f;
				this.GrassValue = 0f;
				this.DemakeValue = 0f;
				this.PalioValue = 0f;
				this.SnakeValue = 1f;
				return;
			}
			this.DirtValue = 0f;
			this.StoneValue = 1f;
			this.SnowValue = 0f;
			this.WoodValue = 0f;
			this.MarbleValue = 0f;
			this.WaterValue = 0f;
			this.FleshValue = 0f;
			this.MetalValue = 0f;
			this.MudValue = 0f;
			this.SecretValue = 0f;
			this.GrassValue = 0f;
			this.DemakeValue = 0f;
			this.PalioValue = 0f;
			this.SnakeValue = 0f;
		}

		public void PlayFootStep()
		{
			base.PlayOneShotEvent("PenitentFootStep", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayJumpSound()
		{
			if (Core.Logic.CurrentState == LogicStates.Unresponsive)
			{
				return;
			}
			base.PlayOneShotEvent("PenitentJump", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRunStopSound()
		{
			if (Core.Logic.CurrentState == LogicStates.Unresponsive)
			{
				return;
			}
			base.PlayOneShotEvent("PenitentRunStop", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDashSound()
		{
			this.StopDashSound();
			base.PlayEvent(ref this._dashEventInstance, "PenitentDash", true);
		}

		public void StopDashSound()
		{
			base.StopEvent(ref this._dashEventInstance);
		}

		public void PlayLandingSound()
		{
			if (Core.Logic.CurrentState == LogicStates.Unresponsive)
			{
				return;
			}
			base.PlayOneShotEvent("PenitentLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayLandingForward()
		{
			base.PlayOneShotEvent("PenitentLandingForward", EntityAudio.FxSoundCategory.Motion);
		}

		public void ClimbLadder()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentClimbLadder");
		}

		public void GrabCliffLede()
		{
			base.PlayOneShotEvent("PenitentHangCliff", EntityAudio.FxSoundCategory.Motion);
		}

		public void ClimbCliffLede()
		{
			base.PlayOneShotEvent("PenitentClimbCliff", EntityAudio.FxSoundCategory.Motion);
		}

		public void JumpOff()
		{
			base.PlayOneShotEvent("PenitentJumpOff", EntityAudio.FxSoundCategory.Motion);
		}

		public void SlidingLadder(out EventInstance eI)
		{
			EventInstance eventInstance = base.AudioManager.CreateCatalogEvent("PenitentSlidingLadder", default(Vector3));
			if (eventInstance.isValid() && !base.Mute)
			{
				eventInstance.start();
			}
			eI = eventInstance;
		}

		public void StopSlidingLadder(EventInstance eventInstance)
		{
			if (!eventInstance.isValid())
			{
				return;
			}
			eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			eventInstance.release();
		}

		public void SlidingLadderLanding()
		{
			base.PlayOneShotEvent("PenitentSlidingLadderLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayIdleModeBlood()
		{
			base.PlayEvent(ref this._idleModeBlood, "PenitentIdleBlood", true);
		}

		public void StopIdleModeBlood()
		{
			base.StopEvent(ref this._idleModeBlood);
		}

		public void PlayIdleModeSword()
		{
			base.PlayOneShotEvent("PenitentIdleSword", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStartDialogue()
		{
			base.PlayOneShotEvent("PenitentStartTalk", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayEndDialogue()
		{
			base.PlayOneShotEvent("PenitentEndTalk", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRangeAttack()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentRangeAttack");
		}

		public void PlayRangeAttackHit()
		{
			base.PlayOneShotEvent("PenitentRangeAttackHit", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayBasicAttack1()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentAttack1");
		}

		public void PlayBasicAttack2()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentAttack2");
		}

		public void PlayHeavyAttack()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentHeavyAttack");
		}

		public void PlayAirAttack1()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentAirAttack1");
		}

		public void PlayAirAttack2()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentAirAttack2");
		}

		public void PlayLoadingChargedAttack()
		{
			if (this._chargedAttackEventInstance.isValid() || base.Mute)
			{
				return;
			}
			this._chargedAttackEventInstance = base.AudioManager.CreateCatalogEvent("PenitentLoadingChargedAttack", default(Vector3));
			this._chargedAttackEventInstance.start();
			this._chargedAttackEventInstance.release();
		}

		public void StopLoadingChargedAttack()
		{
			if (!this._chargedAttackEventInstance.isValid() | base.Mute)
			{
				return;
			}
			this._chargedAttackEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this._chargedAttackEventInstance.release();
			this._chargedAttackEventInstance = default(EventInstance);
		}

		public void UseFlask()
		{
			if (this._useFlaskEventInstance.isValid() || base.Mute)
			{
				return;
			}
			this._useFlaskEventInstance = base.AudioManager.CreateCatalogEvent("UseFlask", default(Vector3));
			this._useFlaskEventInstance.start();
		}

		public void StopUseFlask()
		{
			if (!this._useFlaskEventInstance.isValid() || base.Mute)
			{
				return;
			}
			this._useFlaskEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this._useFlaskEventInstance.release();
			this._useFlaskEventInstance = default(EventInstance);
		}

		public void EmptyFlask()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("EmptyFlask");
		}

		public void HealingExplosion()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("HealingExplosion");
		}

		public void PlayLoadedChargedAttack()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentLoadedChargedAttack");
		}

		public void PlayReleaseChargedAttack()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentReleaseChargedAttack");
		}

		public void PlayStartParry()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentStartParry");
		}

		public void PlayParryHit()
		{
			this.StopParryFx();
			base.PlayEvent(ref this._parryHitEventInstance, "PenitentParryHit", true);
		}

		public void StopParryFx()
		{
			base.StopEvent(ref this._parryHitEventInstance);
		}

		public void PlayParryAttack()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentParry");
		}

		public void PlaySimpleHitToEnemy()
		{
			base.PlayOneShotEvent("PenitentSimpleEnemyHit", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayHeavyHitToEnemy()
		{
			base.PlayOneShotEvent("PenitentHeavyEnemyHit", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayComboHit()
		{
			base.PlayOneShotEvent("PenitentComboHit", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayComboHitUp()
		{
			base.PlayOneShotEvent("PenitentComboHitUp", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayComboHitDown()
		{
			base.PlayOneShotEvent("PenitentComboHitDown", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayDeath()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentDeath");
		}

		public void PlayDeathSpikes()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentDeathBySpike");
		}

		public void PlayDeathFall()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentDeathByFall");
		}

		public void PlayOverthrow()
		{
			if (base.AudioManager == null || !this.Owner.SpriteRenderer.isVisible || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentOverthrow");
		}

		public void PlayPushBack()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentPushBack");
		}

		public void PlaySimpleDamage()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentSimpleEnemyDamage");
		}

		public void PlayHeavyDamage()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentHeavyEnemyDamage");
		}

		public void PlayStickToWall()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.PlayOneShotEvent("PenitentStickToWall", EntityAudio.FxSoundCategory.Climb);
		}

		public void PlayUnHangFromWall()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentUnHangFromWall");
		}

		public void PlayHardLanding()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentHardLanding");
		}

		public void PlayVerticalAttackStart()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentVerticalAttackStart");
		}

		public void PlayVerticalAttackFalling()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentVerticalAttackFalling");
		}

		public void PlayVerticalAttackLanding()
		{
			if (base.Mute)
			{
				return;
			}
			string getLandingFxEventKey = Core.Logic.Penitent.VerticalAttack.GetLandingFxEventKey;
			Core.Audio.EventOneShotPanned(getLandingFxEventKey, base.transform.position);
		}

		public void PlayFinishingComboDown()
		{
			if (base.Mute)
			{
				return;
			}
			Core.Audio.EventOneShotPanned(Core.Logic.Penitent.VerticalAttack.VerticalLandingFxLevel3, base.transform.position);
		}

		public void ActivatePrayer()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			this.StopPrayerCast();
			base.PlayEvent(ref this._prayerEventInstance, "PenitentActivatePrayer", true);
		}

		public void StopPrayerCast()
		{
			base.StopEvent(ref this._prayerEventInstance);
		}

		public void PrayerInvincibility()
		{
			if (base.AudioManager == null || base.Mute)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("PenitentInvincibility");
		}

		public const string ClimbCliffEventKey = "PenitentClimbCliff";

		public const string ClimbLadderEventKey = "PenitentClimbLadder";

		public const string DashEventKey = "PenitentDash";

		public const string FootStepEventKey = "PenitentFootStep";

		public const string HangCliffEventKey = "PenitentHangCliff";

		public const string JumpEventKey = "PenitentJump";

		public const string JumpOffEventKey = "PenitentJumpOff";

		public const string LandingForwardEventKey = "PenitentLandingForward";

		public const string LandingEventKey = "PenitentLanding";

		public const string RunStopEventKey = "PenitentRunStop";

		public const string SlidingLadderEventKey = "PenitentSlidingLadder";

		public const string SlidingLadderLandingEventKey = "PenitentSlidingLadderLanding";

		public const string StickToWallEventKey = "PenitentStickToWall";

		public const string UnHangFromWallEventKey = "PenitentUnHangFromWall";

		public const string HardLanding = "PenitentHardLanding";

		public const string IdleModeBlood = "PenitentIdleBlood";

		public const string IdleModeSword = "PenitentIdleSword";

		public const string StartDialogue = "PenitentStartTalk";

		public const string EndDialogue = "PenitentEndTalk";

		public const string BasicAttack1EventKey = "PenitentAttack1";

		public const string BasicAttack2EventKey = "PenitentAttack2";

		public const string BasicAirAttack1EventKey = "PenitentAirAttack1";

		public const string BasicAirAttack2EventKey = "PenitentAirAttack2";

		public const string PenitentHeavyAttackEventKey = "PenitentHeavyAttack";

		public const string LoadingChargedAttackEventKey = "PenitentLoadingChargedAttack";

		public const string LoadedChargedAttackEventKey = "PenitentLoadedChargedAttack";

		public const string ReleaseChargedAttackEventKey = "PenitentReleaseChargedAttack";

		public const string ParryEventKey = "PenitentParry";

		public const string ParryHitEventKey = "PenitentParryHit";

		public const string StartParryEventKey = "PenitentStartParry";

		public const string VerticalAttackStart = "PenitentVerticalAttackStart";

		public const string VerticalAttackFalling = "PenitentVerticalAttackFalling";

		public const string VerticalAttackLanding = "PenitentVerticalAttackLanding";

		public const string ComboHit = "PenitentComboHit";

		public const string ComboHitUp = "PenitentComboHitUp";

		public const string ComboHitDown = "PenitentComboHitDown";

		public const string RangeAttack = "PenitentRangeAttack";

		public const string RangeAttackDisappear = "PenitentRangeAttackDisappear";

		public const string RangeAttackHit = "PenitentRangeAttackHit";

		public const string RangeAttackFlight = "PenitentRangeAttackFlight";

		public const string SimpleEnemyHitEventKey = "PenitentSimpleEnemyHit";

		public const string PushBackEventKey = "PenitentPushBack";

		public const string HeavyEnemyHitEventKey = "PenitentHeavyEnemyHit";

		public const string CriticalEnemyHitEventKey = "PenitentCriticalEnemyHit";

		public const string SimpleEnemyDamageEventKey = "PenitentSimpleEnemyDamage";

		public const string HeavyEnemyDamageEventKey = "PenitentHeavyEnemyDamage";

		public const string DeathEventKey = "PenitentDeath";

		public const string DeathBySpikesEventKey = "PenitentDeathBySpike";

		public const string DeathByFallEventKey = "PenitentDeathByFall";

		public const string OverthrowEventKey = "PenitentOverthrow";

		public const string UseFlaskEventKey = "UseFlask";

		public const string EmptyFlaskEventKey = "EmptyFlask";

		public const string HealingExplosionEventKey = "HealingExplosion";

		public const string PenitentActivatePrayerEventKey = "PenitentActivatePrayer";

		public const string PrayerInvincibilityEventKey = "PenitentInvincibility";

		private EventInstance _dashEventInstance;

		private EventInstance _idleModeBlood;

		private EventInstance _chargedAttackEventInstance;

		private EventInstance _useFlaskEventInstance;

		private EventInstance _parryHitEventInstance;

		private EventInstance _prayerEventInstance;
	}
}
