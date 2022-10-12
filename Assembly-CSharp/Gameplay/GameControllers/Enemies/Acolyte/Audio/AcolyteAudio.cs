using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Acolyte.Audio
{
	public class AcolyteAudio : EntityAudio
	{
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

		private void SetFloorMaterialParams(EventInstance eventInstance)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Dirt", out parameterInstance);
				parameterInstance.setValue(this.DirtValue);
				ParameterInstance parameterInstance2;
				eventInstance.getParameter("Snow", out parameterInstance2);
				parameterInstance2.setValue(this.SnowValue);
				ParameterInstance parameterInstance3;
				eventInstance.getParameter("Stone", out parameterInstance3);
				parameterInstance3.setValue(this.StoneValue);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
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

		public void PlayFootStep()
		{
			base.PlayOneShotEvent("AcolyteFootstep", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRunning()
		{
			base.PlayOneShotEvent("AcolyteRunning", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStopRunning()
		{
			base.PlayOneShotEvent("AcolyteStopRunning", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayLanding()
		{
			base.PlayOneShotEvent("AcolyteLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayChargeAttack()
		{
			base.PlayOneShotEvent("AcolyteChargeAttack", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayReleaseAttack()
		{
			base.PlayOneShotEvent("AcolyteReleaseAttack", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayBloodDecal()
		{
			base.PlayOneShotEvent("AcolyteBloodDecal", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("AcolyteDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayDeathOnCliffLede()
		{
			base.PlayOneShotEvent("AcolyteDeathOnCliffLede", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayOverthrow()
		{
			base.PlayOneShotEvent("AcolyteOverthrow", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayVaporizationDeath()
		{
			base.PlayOneShotEvent("AcolyteVaporizationDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public const string FootstepEventKey = "AcolyteFootstep";

		public const string RunningEventKey = "AcolyteRunning";

		public const string StopRunningEventKey = "AcolyteStopRunning";

		public const string LandingEventKey = "AcolyteLanding";

		public const string ChargeAttackEventKey = "AcolyteChargeAttack";

		public const string ReleaseAttackEventKey = "AcolyteReleaseAttack";

		public const string BloodDecalEventKey = "AcolyteBloodDecal";

		public const string DeathEventKey = "AcolyteDeath";

		public const string DeathOnCliffLedeEventKey = "AcolyteDeathOnCliffLede";

		public const string OverThrowEventKey = "AcolyteOverthrow";

		public const string VaporizationDeathEventKey = "AcolyteVaporizationDeath";
	}
}
