using System;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Effects.Player.Sprint;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Animator;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.Inventory
{
	public class IncreaseSpeedSwordHeartEffect : ObjectEffect
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
			LevelManager.OnBeforeLevelLoad += this.OnLevelPreload;
		}

		private void OnLevelPreload(Level oldlevel, Level newlevel)
		{
			this.UnsubscribePlayerEvents();
			this.StopUseLoopFx();
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this._penitent = penitent;
			this.SubscribePlayerEvents();
			this.GhostTrail = this._penitent.GetComponentInChildren<GhostTrailGenerator>();
			this.DefaultMotionSettings = this._penitent.Dash.DefaultMoveSetting;
			if (!this.SprintEffect)
			{
				this.SprintEffect = GameObject.FindWithTag("SprintEffect");
			}
			if (this.SprintEffect)
			{
				this.sprintFX = this.SprintEffect.GetComponentInChildren<SprintEffects>();
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this._penitent || !this.isItemEquipped)
			{
				return;
			}
			if (!this._penitent.Status.IsGrounded && this.isEffectApplied)
			{
				this.RemoveEffectFlag = true;
			}
			if (this._penitent.Status.IsGrounded && this.RemoveEffectFlag)
			{
				this.RemoveEffectFlag = false;
				this.ApplyEffect(false);
			}
			if (this._penitent.PlatformCharacterController.IsClimbing)
			{
				this.ApplyEffect(false);
			}
			if (this._penitent.IsGrabbingCliffLede)
			{
				this.ApplyEffect(false);
			}
			if (this._penitent.IsStickedOnWall)
			{
				this.ApplyEffect(false);
			}
			if (this._penitent.Status.IsGrounded && this.isEffectApplied && Mathf.Abs(this._penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed) <= 0.1f)
			{
				this.ApplyEffect(false);
			}
		}

		private void OnFinishDash()
		{
			if (!this.isItemEquipped)
			{
				return;
			}
			if (!this._penitent.Status.IsGrounded)
			{
				return;
			}
			this.ApplyEffect(true);
			this.ActivationSound();
		}

		protected override bool OnApplyEffect()
		{
			this.isItemEquipped = true;
			return true;
		}

		protected override void OnRemoveEffect()
		{
			this.isItemEquipped = false;
			this.ApplyEffect(false);
			base.OnRemoveEffect();
		}

		private void OnLungeAttackStart()
		{
			if (this.isItemEquipped)
			{
				this.ApplyEffect(false);
			}
		}

		private void ApplyEffect(bool apply = true)
		{
			this.isEffectApplied = apply;
			this.ApplySpeedSettings(apply);
			this.ApplyGhostingEffect(apply);
			this.ApplySprintEffect(apply);
			this.ApplySoundLoopFx(apply);
		}

		private void ApplySpeedSettings(bool apply = true)
		{
			if (!this._penitent)
			{
				return;
			}
			float speed = this.MotionSettings.Speed;
			float drag = this.MotionSettings.Drag;
			if (!apply)
			{
				if (Core.InventoryManager.IsRosaryBeadEquipped("RB203"))
				{
					if (this.beadEffect == null)
					{
						this.beadEffect = UnityEngine.Object.FindObjectOfType<IncreaseSpeedBeadEffect>();
					}
					speed = this.beadEffect.BeadMoveSettings.Speed;
					drag = this.beadEffect.BeadMoveSettings.Drag;
				}
				else
				{
					speed = this.DefaultMotionSettings.Speed;
					drag = this.DefaultMotionSettings.Drag;
				}
			}
			this._penitent.PlatformCharacterController.MaxWalkingSpeed = speed;
			this._penitent.PlatformCharacterController.WalkingDrag = drag;
		}

		private void ApplyGhostingEffect(bool apply = true)
		{
			if (!this.GhostTrail)
			{
				return;
			}
			this.GhostTrail.EnableGhostTrail = apply;
		}

		private void ApplySprintEffect(bool apply = true)
		{
			if (!this.SprintEffect)
			{
				this.SprintEffect = GameObject.FindWithTag("SprintEffect");
			}
			if (!this.SprintEffect)
			{
				return;
			}
			this.sprintFX = this.SprintEffect.GetComponentInChildren<SprintEffects>();
			if (this.sprintFX && apply)
			{
				this.sprintFX.EmitOnStart();
			}
		}

		private void ApplySoundLoopFx(bool apply = true)
		{
			if (apply)
			{
				this.StartUseLoopFx();
			}
			else
			{
				this.StopUseLoopFx();
			}
		}

		private void OnStep(object param)
		{
			if (!this.isEffectApplied)
			{
				return;
			}
			Vector3 vector = (Vector3)param;
			float num = 0.75f;
			vector += Vector3.right * (num * (float)((Core.Logic.Penitent.Status.Orientation != EntityOrientation.Right) ? -1 : 1));
			vector += Vector3.up * 0.1f;
			this.PlaySpecialFootStepFx(vector);
			this.sprintFX.EmitFeet(vector);
		}

		private void SubscribePlayerEvents()
		{
			if (!this._penitent || this.SuscribedToTriggerEvents)
			{
				return;
			}
			this.SuscribedToTriggerEvents = true;
			Dash dash = this._penitent.Dash;
			dash.OnFinishDash = (Core.SimpleEvent)Delegate.Combine(dash.OnFinishDash, new Core.SimpleEvent(this.OnFinishDash));
			LungeAttack lungeAttack = this._penitent.LungeAttack;
			lungeAttack.OnLungeAttackStart = (Core.SimpleEvent)Delegate.Combine(lungeAttack.OnLungeAttackStart, new Core.SimpleEvent(this.OnLungeAttackStart));
			PenitentMoveAnimations penitentMoveAnimations = this._penitent.PenitentMoveAnimations;
			penitentMoveAnimations.OnStep = (Core.SimpleEventParam)Delegate.Combine(penitentMoveAnimations.OnStep, new Core.SimpleEventParam(this.OnStep));
		}

		private void UnsubscribePlayerEvents()
		{
			if (!this._penitent || !this.SuscribedToTriggerEvents)
			{
				return;
			}
			this.SuscribedToTriggerEvents = false;
			Dash dash = this._penitent.Dash;
			dash.OnFinishDash = (Core.SimpleEvent)Delegate.Remove(dash.OnFinishDash, new Core.SimpleEvent(this.OnFinishDash));
			LungeAttack lungeAttack = this._penitent.LungeAttack;
			lungeAttack.OnLungeAttackStart = (Core.SimpleEvent)Delegate.Remove(lungeAttack.OnLungeAttackStart, new Core.SimpleEvent(this.OnLungeAttackStart));
			PenitentMoveAnimations penitentMoveAnimations = this._penitent.PenitentMoveAnimations;
			penitentMoveAnimations.OnStep = (Core.SimpleEventParam)Delegate.Remove(penitentMoveAnimations.OnStep, new Core.SimpleEventParam(this.OnStep));
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			LevelManager.OnBeforeLevelLoad -= this.OnLevelPreload;
			this.UnsubscribePlayerEvents();
		}

		private void PlaySpecialFootStepFx(Vector2 position)
		{
			if (!this.isEffectApplied || string.IsNullOrEmpty(this.SpecialFoostepSound))
			{
				return;
			}
			Core.Audio.EventOneShotPanned(this.SpecialFoostepSound, position);
		}

		private void ActivationSound()
		{
			if (this.prayerActivationSoundFx.IsNullOrWhitespace())
			{
				return;
			}
			Core.Audio.PlayOneShot(this.prayerActivationSoundFx, default(Vector3));
		}

		private void StartUseLoopFx()
		{
			if (this._activationSoundLoopInstance.isValid() || this.prayerUseLoopSoundFx.IsNullOrWhitespace())
			{
				return;
			}
			this._activationSoundLoopInstance = Core.Audio.CreateEvent(this.prayerUseLoopSoundFx, default(Vector3));
			this._activationSoundLoopInstance.start();
		}

		private void StopUseLoopFx()
		{
			if (!this._activationSoundLoopInstance.isValid())
			{
				return;
			}
			this._activationSoundLoopInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this._activationSoundLoopInstance.release();
			this._activationSoundLoopInstance = default(EventInstance);
		}

		[EventRef]
		public string SpecialFoostepSound;

		private Penitent _penitent;

		private GhostTrailGenerator GhostTrail;

		public Dash.MoveSetting MotionSettings;

		private Dash.MoveSetting DefaultMotionSettings;

		private bool isEffectApplied;

		private bool isItemEquipped;

		private bool RemoveEffectFlag;

		private bool SuscribedToTriggerEvents;

		private SprintEffects sprintFX;

		private GameObject SprintEffect;

		private IncreaseSpeedBeadEffect beadEffect;

		[EventRef]
		public string prayerActivationSoundFx;

		[EventRef]
		public string prayerUseLoopSoundFx;

		private EventInstance _activationSoundLoopInstance;
	}
}
