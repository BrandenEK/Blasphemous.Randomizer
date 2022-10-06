using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WheelCarrier.Audio
{
	public class WheelCarrierAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		public void PlayStepLeft()
		{
			base.PlayOneShotEvent("RocieroFootStepLeft", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayStepRight()
		{
			base.PlayOneShotEvent("RocieroFootStepRight", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayAttack()
		{
			base.PlayEvent(ref this._attackEventInstance, "RocieroAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void SetAttackMoveParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
		}

		public void PlayDeath()
		{
			this.StopAttack();
			base.PlayOneShotEvent("RocieroDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void SetMoveParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Moves", ref parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private const string StepLeftEventKey = "RocieroFootStepLeft";

		private const string StepRightEventKey = "RocieroFootStepRight";

		private const string DeathEventKey = "RocieroDeath";

		private const string AttackEventKey = "RocieroAttack";

		private const string MoveParameterKey = "Moves";

		private EventInstance _attackEventInstance;

		private EventInstance _chasingEventInstance;

		private EventInstance _walkEventInstance;
	}
}
