using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	public class BurntFaceAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				this.StopAll();
			}
		}

		public void PlayAppear()
		{
			base.PlayOneShotEvent("BurntFaceAppear", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDeath()
		{
			base.PlayOneShotEvent("BurntFaceDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayDisappear()
		{
			base.PlayOneShotEvent("BurntFaceAppear", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlayBeamCharge(ref EventInstance instance)
		{
			base.StopEvent(ref instance);
			base.PlayEvent(ref instance, "BurntFaceBeamFire", true);
		}

		public void PlayBeamFire(ref EventInstance instance)
		{
			this.SetParam(instance, "Attack", 1f);
		}

		public void StopBeamFire(ref EventInstance e)
		{
			this.SetParam(e, "End", 1f);
		}

		public void StopAll()
		{
		}

		public void SetParam(EventInstance eventInstance, string paramKey, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter(paramKey, out parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private const string AppearEventKey = "BurntFaceAppear";

		private const string DisappearEventKey = "BurntFaceDisappear";

		private const string BeamFireEventKey = "BurntFaceBeamFire";

		private const string BeamChargeEventKey = "BurntFaceBeamCharge";

		private const string DeathEventKey = "BurntFaceDeath";

		private const string AttackParamKey = "Attack";

		private const string EndParamKey = "End";
	}
}
