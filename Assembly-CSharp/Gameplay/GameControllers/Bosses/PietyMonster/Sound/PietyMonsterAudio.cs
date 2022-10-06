using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Sound
{
	public class PietyMonsterAudio : EntityAudio
	{
		protected override void OnWake()
		{
			base.OnWake();
			this.EventInstances = new List<EventInstance>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (this.FloorCollider != null)
			{
				this.FloorSensorEmitter = this.FloorCollider.GetComponent<ICollisionEmitter>();
			}
			if (this.WeaponCollider != null)
			{
				this.WeaponSensorEmitter = this.WeaponCollider.GetComponent<ICollisionEmitter>();
			}
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

		public void PlayWalk()
		{
			base.PlayOneShotEvent("PietatWalk", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStepStomp()
		{
			base.PlayOneShotEvent("PietatStep", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayTurn()
		{
			base.PlayOneShotEvent("PietatTurn", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStop()
		{
			base.PlayOneShotEvent("PietatStop", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySlash()
		{
			base.PlayOneShotEvent("PietatSlash", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStomp()
		{
			base.PlayOneShotEvent("PietatStomp", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDead()
		{
			base.PlayOneShotEvent("PietatDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayScream()
		{
			base.PlayOneShotEvent("PietatSmashScream", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlaySmash()
		{
			base.PlayOneShotEvent("PietatSmash", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayStompHit()
		{
			base.PlayOneShotEvent("PietatStompHit", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayGetUp()
		{
			base.PlayOneShotEvent("PietatSmashGetUp", EntityAudio.FxSoundCategory.Attack);
		}

		public void ReadyToSpit()
		{
			base.PlayOneShotEvent("PietatReadyToSpit", EntityAudio.FxSoundCategory.Attack);
		}

		public void Spit()
		{
			base.PlayOneShotEvent("PietatSpit", EntityAudio.FxSoundCategory.Attack);
		}

		public void RootAttack()
		{
			base.PlayOneShotEvent("PietatRootAttack", EntityAudio.FxSoundCategory.Attack);
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

		public const string WalkEventKey = "PietatWalk";

		public const string TurnEventKey = "PietatTurn";

		public const string StopEventKey = "PietatStop";

		public const string StepEventKey = "PietatStep";

		public const string SlashEventKey = "PietatSlash";

		public const string StompEventKey = "PietatStomp";

		public const string SmashEventKey = "PietatSmash";

		public const string SmashScreamEventKey = "PietatSmashScream";

		public const string SmashGetUpEventKey = "PietatSmashGetUp";

		public const string ReadyToSpitEventKey = "PietatReadyToSpit";

		public const string SpitEventKey = "PietatSpit";

		public const string SpitExplosionEventKey = "PietatSpitExplosion";

		public const string SpitGrowEventKey = "PietatSpitGrow";

		public const string SpitHitEventKey = "PietatSpitHit";

		public const string SpitDestroyEventKey = "PietatSpitDestroyHit";

		public const string PietatRootAttackEventKey = "PietatRootAttack";

		public const string PietatStompHitEventKey = "PietatStompHit";

		public const string DeathEventKey = "PietatDeath";
	}
}
