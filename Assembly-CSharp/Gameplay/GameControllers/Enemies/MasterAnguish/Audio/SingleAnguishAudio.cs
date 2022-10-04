using System;
using FMOD.Studio;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MasterAnguish.Audio
{
	public class SingleAnguishAudio : EntityAudio
	{
		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.Owner.SpriteRenderer.isVisible || this.Owner.Status.Dead)
			{
				this.StopAll();
			}
		}

		public void PlayLanceCharge()
		{
			base.PlayOneShotEvent("LanceChargeKey", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayMace()
		{
			base.StopEvent(ref this._maceInstance);
			base.PlayEvent(ref this._maceInstance, "AngustiasThrow", true);
		}

		public void StopMace()
		{
			this.SetEndParam(this._maceInstance, 1f);
		}

		public void PlayLanceShot()
		{
			base.PlayOneShotEvent("LanceShotKey", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySpawn()
		{
			base.PlayOneShotEvent("SpawnEventKey", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayAppear()
		{
			base.PlayOneShotEvent("AngustiasAppear", EntityAudio.FxSoundCategory.Motion);
		}

		public void StopAll()
		{
		}

		public void SetEndParam(EventInstance eventInstance, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("End", out parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private const string LanceChargeKey = "LanceChargeKey";

		private const string LanceShotKey = "LanceShotKey";

		private const string SpawnEventKey = "SpawnEventKey";

		private const string AppearEventKey = "AngustiasAppear";

		private const string MaceThrowKey = "AngustiasThrow";

		private const string EndParameterKey = "End";

		private EventInstance _maceInstance;
	}
}
