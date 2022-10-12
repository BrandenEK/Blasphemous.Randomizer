using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Processioner.Audio
{
	public class ProcesionerAudio : EntityAudio
	{
		public void PlayAttack()
		{
			this.StopAttack();
			base.PlayEvent(ref this._attackEventInstance, "ProcessionerAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void PlayWalk()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("ProcessionerFootsteps", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("ProcessionerDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void SetAttackParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
		}

		public void StartChargeLoop()
		{
			base.PlayEvent(ref this._chargeLoopEventInstance, "FireChargeLoop", true);
		}

		public void StopChargeLoop()
		{
			base.StopEvent(ref this._chargeLoopEventInstance);
		}

		public void SetMoveParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Moves", out parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private const string WalkEventKey = "ProcessionerFootsteps";

		private const string AttackEventKey = "ProcessionerAttack";

		private const string DeathEventKey = "ProcessionerDeath";

		private const string ChargeLoop = "FireChargeLoop";

		private EventInstance _attackEventInstance;

		private EventInstance _chargeLoopEventInstance;

		private const string MoveParameterKey = "Moves";
	}
}
