using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce.Audio
{
	public class QuirceAudio : EntityAudio
	{
		public void PlayDeath()
		{
			base.PlayOneShotEvent("QuirceDeath", EntityAudio.FxSoundCategory.Damage);
		}

		public void PlaySwordlessDash()
		{
			base.PlayOneShotEvent("QuirceSwordlessDash", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayTeleportOut()
		{
			base.PlayOneShotEvent("QuirceTeleportOut", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayTeleportIn()
		{
			base.PlayOneShotEvent("QuirceTeleportIn", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayPlunge()
		{
			base.PlayOneShotEvent("QuircePlunge", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayToss()
		{
			base.PlayOneShotEvent("QuirceSwordToss", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayPreDash()
		{
			base.PlayOneShotEvent("QuircePreDash", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayHitWall()
		{
			base.PlayOneShotEvent("QuirceSwordHitsWall", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayBigDash()
		{
			base.PlayOneShotEvent("QuirceBigDash", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySpinSword()
		{
			this.StopSpinSword();
			base.PlayEvent(ref this._spiningSword, "QuirceThrowSword", true);
		}

		public void StopSpinSword()
		{
			base.StopEvent(ref this._spiningSword);
		}

		public void EndSwordSpinSound()
		{
			this.SetEndParam(this._spiningSword, 1f);
		}

		private void StopAll()
		{
			this.StopSpinSword();
		}

		private void SetEndParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("end", out parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private void OnDestroy()
		{
			this.StopAll();
		}

		private const string SwordlessDashEventKey = "QuirceSwordlessDash";

		private const string DeathEventKey = "QuirceDeath";

		private const string TeleportOutEventKey = "QuirceTeleportOut";

		private const string TeleportInEventKey = "QuirceTeleportIn";

		private const string PlungeEventKey = "QuircePlunge";

		private const string PreDashEventKey = "QuircePreDash";

		private const string BigDashEventKey = "QuirceBigDash";

		private const string SwordTossEventKey = "QuirceSwordToss";

		private const string SwordStuckEventKey = "QuirceSwordHitsWall";

		private const string SpinSwordEventKey = "QuirceThrowSword";

		private const string endParamKey = "end";

		private EventInstance _spiningSword;
	}
}
