using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Flagellant.Audio
{
	public class FlagellantAudio : EntityAudio
	{
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
			catch
			{
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
			this.Owner.OnDeath += this.OwnerOnDeath;
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

		private void OwnerOnDeath()
		{
			this.DeathValue = 1f;
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
			base.PlayOneShotEvent("FlagellantFootStep", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayRunning()
		{
			base.PlayOneShotEvent("FlagellantRunning", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayLandingSound()
		{
			base.PlayOneShotEvent("FlagellantLanding", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayBasicAttack()
		{
			base.PlayOneShotEvent("FlagellantBasicAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAttackHit()
		{
			base.PlayOneShotEvent("FlagellantAttackHit", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySelfLash()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantSelfLash");
		}

		public void PlayBloodDecal()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantBloodDecal");
		}

		public void PlayDeath()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantDeath");
		}

		public void PlayOverThrow()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantOverthrow");
		}

		public void PlayVaporizationDeath()
		{
			if (base.AudioManager == null)
			{
				return;
			}
			base.AudioManager.PlaySfxOnCatalog("FlagellantVaporizationDeath");
		}

		public const string FootStepEventKey = "FlagellantFootStep";

		public const string RunnigEventKey = "FlagellantRunning";

		public const string LandingEventKey = "FlagellantLanding";

		public const string BasicAttackEventKey = "FlagellantBasicAttack";

		public const string BasicAttackHitEventKey = "FlagellantAttackHit";

		public const string SelfLashEventKey = "FlagellantSelfLash";

		public const string BloodDecalEventKey = "FlagellantBloodDecal";

		public const string DeathEventKey = "FlagellantDeath";

		public const string OverthrowEventKey = "FlagellantOverthrow";

		public const string VaporizationDeathEventKey = "FlagellantVaporizationDeath";

		private EventInstance _basicAttackEventInstance;
	}
}
