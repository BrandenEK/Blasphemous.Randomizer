using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.BellCarrier.Animator;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellCarrier.Audio
{
	public class BellCarrierAudio : EntityAudio
	{
		private void SetFloorMaterialParams(EventInstance eventInstance)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Dirt", ref parameterInstance);
				parameterInstance.setValue(this.DirtValue);
				ParameterInstance parameterInstance2;
				eventInstance.getParameter("Snow", ref parameterInstance2);
				parameterInstance2.setValue(this.SnowValue);
				ParameterInstance parameterInstance3;
				eventInstance.getParameter("Stone", ref parameterInstance3);
				parameterInstance3.setValue(this.StoneValue);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private void OnDestroy()
		{
			if (this.FloorSensorEmitter != null)
			{
				this.FloorSensorEmitter.OnStay -= this.FloorSensorListenerOnStay;
				this.FloorSensorEmitter.OnExit -= this.FloorSensorListenerOnExit;
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
				this.FloorSensorEmitter.OnStay += this.FloorSensorListenerOnStay;
				this.FloorSensorEmitter.OnExit += this.FloorSensorListenerOnExit;
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
			this._animatorInyector = this.Owner.GetComponentInChildren<BellCarrierAnimatorInyector>();
		}

		private void WeaponSensorEmitterOnExit(object sender, Collider2DParam e)
		{
			if ((this.WeaponLayerMask.value & 1 << e.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
		}

		private void WeaponSensorEmitterOnStay(object sender, Collider2DParam e)
		{
			if ((this.WeaponLayerMask.value & 1 << e.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
			this.SetParametersValuesByEnemy(e.Collider2DArg);
		}

		private void FloorSensorListenerOnExit(object sender, Collider2DParam e)
		{
			if ((this.FloorLayerMask.value & 1 << e.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
		}

		private void FloorSensorListenerOnStay(object sender, Collider2DParam e)
		{
			if ((this.FloorLayerMask.value & 1 << e.Collider2DArg.gameObject.layer) <= 0)
			{
				return;
			}
			this.SetParameterValuesByFloor(e.Collider2DArg);
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
			if (tag != null)
			{
				if (tag == "Material:Dirt")
				{
					this.DirtValue = 1f;
					this.StoneValue = 0f;
					this.SnowValue = 0f;
					return;
				}
				if (tag == "Material:Stone")
				{
					this.DirtValue = 0f;
					this.StoneValue = 1f;
					this.SnowValue = 0f;
					return;
				}
				if (tag == "Material:Snow")
				{
					this.DirtValue = 0f;
					this.StoneValue = 0f;
					this.SnowValue = 1f;
					return;
				}
			}
			this.DirtValue = 0f;
			this.StoneValue = 1f;
			this.SnowValue = 0f;
		}

		public void PlayRun()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierRun", EntityAudio.FxSoundCategory.Motion);
		}

		public void DropBell()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierDropBell", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRunStop()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierRunStop", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStartToRun()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierStartToRun", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayTurnAround()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierTurnAround", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayTurnAroundRun()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierTurnAroundRun", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayBellCarrierWallCrush()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierWallCrush", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("BellCarrierDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayWakeUp()
		{
			if (!this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			base.PlayOneShotEvent("BellCarrierWakeUp", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDamageSound()
		{
			Hit lastHit = this.Owner.EntityDamageArea.LastHit;
			if (this._animatorInyector.CurrentDamageSoundType == BellCarrierAnimatorInyector.DamageSoundType.AfterTurnAround)
			{
				this.PlayBellCarrierFrontHit();
			}
			else
			{
				this.PlayDamageSoundByHit(lastHit);
			}
		}

		public void PlayBellCarrierFrontHit()
		{
			base.PlayOneShotEvent("BellCarrierFrontHit", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDamageSoundByHit(Hit hit)
		{
			Core.Audio.EventOneShotPanned(hit.HitSoundId, this.Owner.transform.position);
		}

		private BellCarrierAnimatorInyector _animatorInyector;

		public const string RunEventKey = "BellCarrierRun";

		public const string RunStopEventKey = "BellCarrierRunStop";

		public const string StartToRunEventKey = "BellCarrierStartToRun";

		public const string TurnAroundEventKey = "BellCarrierTurnAround";

		public const string TurnAroundRunEventKey = "BellCarrierTurnAroundRun";

		public const string DropBellEventKey = "BellCarrierDropBell";

		public const string WakeUpEventKey = "BellCarrierWakeUp";

		public const string FrontHitEventKey = "BellCarrierFrontHit";

		public const string DeathEventKey = "BellCarrierDeath";

		public const string WallCrashEventKey = "BellCarrierWallCrush";
	}
}
