using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Menina
{
	public class MeninaAudio : EntityAudio
	{
		public virtual void PlayDeath()
		{
			base.PlayOneShotEvent("MeninaDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public virtual void PlayAttack()
		{
			base.PlayEvent(ref this._attackSoundInstance, "MeninaAttack", true);
		}

		public virtual void StopAttack()
		{
			base.StopEvent(ref this._attackSoundInstance);
		}

		public virtual void PlayLeftLeg()
		{
			base.PlayOneShotEvent("MeninaFootStepLeft", EntityAudio.FxSoundCategory.Motion);
		}

		public virtual void PlayRightLeg()
		{
			base.PlayOneShotEvent("MeninaFootStepRight", EntityAudio.FxSoundCategory.Motion);
		}

		public virtual void PlayIdle()
		{
			if (this.Owner.SpriteRenderer.isVisible && !this.Owner.Status.Dead)
			{
				base.PlayOneShotEvent("MeninaIdle", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void SetAttackMoveParam(float value)
		{
			this.SetMoveParam(this._attackSoundInstance, value);
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

		private const string IdleEventKey = "MeninaIdle";

		private const string DeathEventKey = "MeninaDeath";

		private const string AttackEventKey = "MeninaAttack";

		private const string LeftLegEventKey = "MeninaFootStepLeft";

		private const string RightLegEventKey = "MeninaFootStepRight";

		protected EventInstance _attackSoundInstance;

		private const string MoveParameterKey = "Moves";
	}
}
