using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Legionary.Audio
{
	public class LegionaryAudio : EntityAudio
	{
		public void PlayWalk_AUDIO()
		{
			base.PlayOneShotEvent("LegionarioRun", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayLightAttack_AUDIO()
		{
			base.PlayOneShotEvent("LegionarioNormalAttack", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySlideAttack_AUDIO()
		{
			this.StopSlideAttack_AUDIO();
			base.PlayEvent(ref this._slideAttackInstance, "LegionarioSlideAttack", true);
		}

		public void StopSlideAttack_AUDIO()
		{
			base.StopEvent(ref this._slideAttackInstance);
		}

		public void PlayDeath_AUDIO()
		{
			base.PlayOneShotEvent("LegionarioDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlayGroundHit_AUDIO()
		{
			base.PlayOneShotEvent("LegionarioGroundHit", EntityAudio.FxSoundCategory.Motion);
		}

		public void SetAttackParam(float value)
		{
			this.SetMoveParam(this._slideAttackInstance, value);
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

		private void OnDestroy()
		{
			this.StopSlideAttack_AUDIO();
		}

		public const string FootStepEventKey = "LegionarioRun";

		public const string SlideAttackEventKey = "LegionarioSlideAttack";

		public const string LightAttackEventKey = "LegionarioNormalAttack";

		public const string DeathEventKey = "LegionarioDeath";

		public const string ThunderEventKey = "LegionarioThunder";

		public const string GroundHitEventKey = "LegionarioGroundHit";

		private const string MoveParameterKey = "End";

		private EventInstance _slideAttackInstance;
	}
}
