using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Audio
{
	public class MeltedLadyAudio : EntityAudio
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.Owner.OnDeath += this.OnDeath;
		}

		private void OnDeath()
		{
			this.StopAttack();
		}

		public void PlayAttack()
		{
			this.StopAttack();
			base.PlayEvent(ref this._attackEventInstance, "MeltedLadyAttack", true);
		}

		public void Hurt()
		{
			base.PlayOneShotEvent("MeltedLadyHit", EntityAudio.FxSoundCategory.Damage);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void Appearing()
		{
			base.PlayOneShotEvent("MeltedLadyAppearing", EntityAudio.FxSoundCategory.Motion);
		}

		public void Disappearing()
		{
			base.PlayOneShotEvent("MeltedLadyDisappearing", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("MeltedLadyDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void SetAttackParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
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

		private const string AttackEventKey = "MeltedLadyAttack";

		private const string AppearingEventKey = "MeltedLadyAppearing";

		private const string DisappearingEventKey = "MeltedLadyDisappearing";

		private const string DeathEventKey = "MeltedLadyDeath";

		private const string MeltedLadyDamage = "MeltedLadyHit";

		private EventInstance _attackEventInstance;

		private const string MoveParameterKey = "Moves";
	}
}
