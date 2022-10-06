using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffHusk.Audio
{
	public class PontiffHuskBossAudio : EntityAudio
	{
		private void Awake()
		{
			this.Owner = base.GetComponent<PontiffHuskBoss>();
		}

		public void PlayVanishIn()
		{
			this.PlayOneShot_AUDIO("PontiffHuskVanishIn", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayVanishOut()
		{
			this.PlayOneShot_AUDIO("PontiffHuskVanishOut", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayExecution()
		{
			this.PlayOneShot_AUDIO("PontiffHuskExecution", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayTurnAround()
		{
			this.PlayOneShot_AUDIO("PontiffHuskTurnAround", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlayChargedBlast()
		{
			this.PlayLocalized_AUDIO("PontiffHuskChargedBlast");
		}

		public void PlayChargedBlastNoVoice()
		{
			this.PlayLocalized_AUDIO("PontiffHuskChargedBlastNoVoice");
		}

		public void PlayAmbientPostCombat()
		{
			this.Play_AUDIO("AmbientPostPontiffCombat");
		}

		public void StopAmbientPostCombat()
		{
			this.Stop_AUDIO("AmbientPostPontiffCombat");
		}

		public void StartCombatMusic()
		{
			Core.Audio.Ambient.SetSceneParam("Combat", 1f);
		}

		public void PlayLocalized_AUDIO(string eventId)
		{
			EventInstance panning = base.AudioManager.CreateCatalogEvent(eventId, default(Vector3));
			if (!panning.isValid())
			{
				Debug.LogError(string.Format("ERROR: Couldn't find catalog sound event called <{0}>", eventId));
				return;
			}
			float num = (!Core.Localization.GetCurrentAudioLanguageCode().ToUpper().StartsWith("ES")) ? 0f : 1f;
			panning.setParameterValue("spanish", num);
			panning.setCallback(base.SetPanning(panning), 1);
			panning.start();
			panning.release();
		}

		public void PlayOneShot_AUDIO(string eventId, EntityAudio.FxSoundCategory category = EntityAudio.FxSoundCategory.Attack)
		{
			base.PlayOneShotEvent(eventId, category);
		}

		public void Play_AUDIO(string eventId)
		{
			EventInstance value;
			if (this.eventRefsByEventId.TryGetValue(eventId, out value))
			{
				base.StopEvent(ref value);
				this.eventRefsByEventId.Remove(eventId);
			}
			value = default(EventInstance);
			base.PlayEvent(ref value, eventId, false);
			this.eventRefsByEventId[eventId] = value;
		}

		public void Stop_AUDIO(string eventId)
		{
			EventInstance eventInstance;
			if (!this.eventRefsByEventId.TryGetValue(eventId, out eventInstance))
			{
				return;
			}
			base.StopEvent(ref eventInstance);
			this.eventRefsByEventId.Remove(eventId);
		}

		public void StopAll()
		{
			foreach (string key in this.eventRefsByEventId.Keys)
			{
				EventInstance eventInstance = this.eventRefsByEventId[key];
				base.StopEvent(ref eventInstance);
			}
			this.eventRefsByEventId.Clear();
		}

		private Dictionary<string, EventInstance> eventRefsByEventId = new Dictionary<string, EventInstance>();

		private const string PontiffHusk_VanishIn = "PontiffHuskVanishIn";

		private const string PontiffHusk_VanishOut = "PontiffHuskVanishOut";

		private const string PontiffHusk_Execution = "PontiffHuskExecution";

		private const string PontiffHusk_TurnAround = "PontiffHuskTurnAround";

		private const string PontiffHusk_ChargedBlast = "PontiffHuskChargedBlast";

		private const string PontiffHusk_ChargedBlastNoVoice = "PontiffHuskChargedBlastNoVoice";

		private const string PontiffHusk_AmbientPostCombat = "AmbientPostPontiffCombat";

		private const string CombatParameter = "Combat";
	}
}
