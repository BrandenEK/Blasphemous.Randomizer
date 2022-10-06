using System;
using FMOD.Studio;
using Gameplay.GameControllers.Bosses.BossFight;
using Gameplay.GameControllers.Entities.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras.Audio
{
	public class EsdrasAudio : EntityAudio
	{
		public void PlayQueuedEvent()
		{
			if (string.IsNullOrEmpty(this.queuedEventKey))
			{
				return;
			}
			this.StopAll();
			base.PlayEvent(ref this.queuedEvent, this.queuedEventKey, true);
		}

		public void QueueSlideAttack_AUDIO()
		{
			this.queuedEventKey = "EsdrasSlideAttack";
		}

		public void StopSlideAttack_AUDIO()
		{
			this.SetEndParam(1f);
		}

		public void QueueSpinLoop()
		{
			this.queuedEventKey = "EsdrasSpinLoop";
		}

		public void StopSpinLoop_AUDIO()
		{
			this.SetEndParam(1f);
		}

		public void QueueSpinProjectile()
		{
			this.queuedEventKey = "EsdrasSpinProjectile";
		}

		public void StopSpinProjectile_AUDIO()
		{
			this.SetEndParam(1f);
		}

		public void ChangeEsdrasMusic()
		{
			BossFightAudio bossFightAudio = Object.FindObjectOfType<BossFightAudio>();
			bossFightAudio.SetBossTrackParam("Mixdown", 1f);
		}

		public void PlayDeath_AUDIO()
		{
			this.StopAll();
			base.PlayOneShotEvent("EsdrasDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayGroundHit_AUDIO()
		{
			base.PlayOneShotEvent("EsdrasGroundHit", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayCallSister_AUDIO()
		{
			base.PlayOneShotEvent("EsdrasPerpetuaCall", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayLightAttack_AUDIO()
		{
			base.PlayOneShotEvent("EsdrasNormalAttack", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayHeavyAttackSmash_AUDIO()
		{
			base.PlayOneShotEvent("EsdrasThunderAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayFootStep_AUDIO()
		{
			base.PlayOneShotEvent("EsdrasRun", EntityAudio.FxSoundCategory.Motion);
		}

		public void SetEndParam(float value)
		{
			if (this.queuedEvent.isValid())
			{
				this.SetMoveParam(this.queuedEvent, value);
			}
		}

		[Button("Stop events", 0)]
		public void StopAll()
		{
			base.StopEvent(ref this.queuedEvent);
		}

		private void OnDestroy()
		{
			this.StopAll();
		}

		private void SetMoveParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("End", ref parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private const string ESDRAS_DEATH = "EsdrasDeath";

		private const string ESDRAS_NORMAL_ATTACK = "EsdrasNormalAttack";

		private const string ESDRAS_GROUND_HIT = "EsdrasGroundHit";

		private const string ESDRAS_FOOTSTEP = "EsdrasRun";

		private const string ESDRAS_HEAVY_ATTACK_SMASH = "EsdrasThunderAttack";

		public const string ESDRAS_SLIDE_ATTACK = "EsdrasSlideAttack";

		public const string ESDRAS_SPIN_LOOP = "EsdrasSpinLoop";

		public const string ESDRAS_SPINPROJECTILE = "EsdrasSpinProjectile";

		public const string ESDRAS_CALL_SISTER = "EsdrasPerpetuaCall";

		private EventInstance _slideAttackInstance;

		private EventInstance _spinLoopInstance;

		private EventInstance _spinProjectileInstance;

		private const string MoveParameterKey = "End";

		private EventInstance queuedEvent;

		private string queuedEventKey;
	}
}
