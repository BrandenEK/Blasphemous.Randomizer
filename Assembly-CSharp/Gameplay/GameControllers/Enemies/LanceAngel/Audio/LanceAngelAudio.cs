using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.LanceAngel.Audio
{
	public class LanceAngelAudio : EntityAudio
	{
		public void PlayFlap()
		{
			if (this.Owner.SpriteRenderer.isVisible)
			{
				base.PlayOneShotEvent("AngelFly", EntityAudio.FxSoundCategory.Motion);
			}
		}

		public void PlayAttack()
		{
			this.StopAttack();
			base.PlayEvent(ref this._attackEventInstance, "AngelAttack", true);
		}

		public void StopAttack()
		{
			base.StopEvent(ref this._attackEventInstance);
		}

		public void SetAttackParam(float value)
		{
			this.SetMoveParam(this._attackEventInstance, value);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("AngelDeath", EntityAudio.FxSoundCategory.Damage);
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

		private void OnDestroy()
		{
			this.StopAttack();
		}

		public const string FlyEventKey = "AngelFly";

		public const string AttackEventKey = "AngelAttack";

		public const string DeathEventKey = "AngelDeath";

		private const string MoveParameterKey = "Moves";

		private EventInstance _attackEventInstance;
	}
}
