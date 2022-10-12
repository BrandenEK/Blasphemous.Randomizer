using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.FlyingPortrait.Audio
{
	public class FlyingPortraitAudio : EntityAudio
	{
		public void PlayUnlock()
		{
			base.PlayOneShotEvent("FlyingPortraitUnlock", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayFloating()
		{
			base.PlayEvent(ref this._floatingEventInstance, "FlyingPortraitFloating", true);
		}

		public void PlayDeath()
		{
			this.StopAttack();
			base.StopEvent(ref this._floatingEventInstance);
			base.PlayOneShotEvent("FlyingPortraitDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayAttack()
		{
			base.PlayEvent(ref this._attackEventInstance, "FlyingPortraitAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
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

		private const string AttackEventKey = "FlyingPortraitAttack";

		private const string FloatingEventKey = "FlyingPortraitFloating";

		private const string DeathEventKey = "FlyingPortraitDeath";

		private const string UnlockEventKey = "FlyingPortraitUnlock";

		private EventInstance _attackEventInstance;

		private EventInstance _floatingEventInstance;

		private const string MoveParameterKey = "Moves";
	}
}
