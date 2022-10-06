using System;
using System.Collections.Generic;
using FMOD.Studio;
using Gameplay.GameControllers.Bosses.BossFight;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake.Audio
{
	public class SnakeAudio : EntityAudio
	{
		private void Awake()
		{
			this.Owner = base.GetComponent<Snake>();
		}

		public void PlaySnakeRain()
		{
			this.Play_AUDIO("AquilonRain");
		}

		public void StopSnakeRain()
		{
			this.Stop_AUDIO("AquilonRain");
		}

		public void IncreaseSnakeRainState()
		{
			EventInstance eventInstance;
			if (this.eventRefsByEventId.TryGetValue("AquilonRain", out eventInstance))
			{
				this.fightState++;
				string text = "State" + this.fightState;
				eventInstance.setParameterValue(text, 1f);
				this.SetBossTrackParam(this.fightState);
			}
		}

		public void SetSnakeRainState(int state)
		{
			EventInstance eventInstance;
			if (this.eventRefsByEventId.TryGetValue("AquilonRain", out eventInstance))
			{
				for (int i = 1; i < 4; i++)
				{
					float num = (i > state) ? 0f : 1f;
					string text = "State" + i;
					eventInstance.setParameterValue(text, num);
				}
			}
		}

		public void PlaySnakeVanishOut()
		{
			this.PlayOneShot_AUDIO("SnakeVanishOut", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeVanishIn()
		{
			this.PlayOneShot_AUDIO("SnakeVanishIn", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakePhaseMovement()
		{
			this.PlayOneShot_AUDIO("SnakePhaseMovement", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeTailShot()
		{
			this.PlayOneShot_AUDIO("SnakeTailShot", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySnakeTail()
		{
			this.PlayOneShot_AUDIO("SnakeTail", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeThunder()
		{
			this.PlayOneShot_AUDIO("Thunder", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeTongueExplosion()
		{
			this.PlayOneShot_AUDIO("SnakeTongleExplosion", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeWind()
		{
			this.PlayOneShot_AUDIO("SnakeWind", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeDeath()
		{
			this.PlayOneShot_AUDIO("SnakeDeath", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeDeathStinger()
		{
			this.PlayOneShot_AUDIO("SnakeDeathStinger", EntityAudio.FxSoundCategory.Motion);
		}

		public void PlaySnakeElectricTail()
		{
			this.Play_AUDIO("SnakeElectricTail");
		}

		public void StopSnakeElectricTail()
		{
			this.Stop_AUDIO("SnakeElectricTail");
		}

		public void PlaySnakeElectricShot()
		{
			this.Play_AUDIO("SnakeElectricShot");
		}

		public void StopSnakeElectricShot()
		{
			this.Stop_AUDIO("SnakeElectricShot");
		}

		public void SetBossTrackParam(int state)
		{
			if (this.BossFightAudio == null)
			{
				this.BossFightAudio = Object.FindObjectOfType<BossFightAudio>();
			}
			this.BossFightAudio.SetBossTrackParam("State" + state, 1f);
		}

		public void PlaySnakeGrunt1()
		{
			this.PlayOneShot_AUDIO("SnakeGrunt_#1", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySnakeGrunt2()
		{
			this.PlayOneShot_AUDIO("SnakeGrunt_#2", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySnakeGrunt3()
		{
			this.PlayOneShot_AUDIO("SnakeGrunt_#3", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySnakeGrunt4()
		{
			this.PlayOneShot_AUDIO("SnakeGrunt_#4", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySnakeBite()
		{
			this.PlayOneShot_AUDIO("SnakeBite", EntityAudio.FxSoundCategory.Attack);
		}

		public void PlaySnakeBack()
		{
			this.PlayOneShot_AUDIO("SnakeBack", EntityAudio.FxSoundCategory.Motion);
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

		public void SetParam(EventInstance eventInstance, string paramKey, float value)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter(paramKey, ref parameterInstance);
				parameterInstance.setValue(value);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		public BossFightAudio BossFightAudio;

		private Dictionary<string, EventInstance> eventRefsByEventId = new Dictionary<string, EventInstance>();

		private const string Snake_Rain = "AquilonRain";

		private const string Snake_Grunt1 = "SnakeGrunt_#1";

		private const string Snake_Grunt2 = "SnakeGrunt_#2";

		private const string Snake_Grunt3 = "SnakeGrunt_#3";

		private const string Snake_Grunt4 = "SnakeGrunt_#4";

		private const string Sanake_ClosingMouth = "SnakeClosingMouth";

		private const string Snake_Bite = "SnakeBite";

		private const string Snake_Back = "SnakeBack";

		private const string Snake_VanishOut = "SnakeVanishOut";

		private const string Snake_VanishIn = "SnakeVanishIn";

		private const string Snake_PhaseMovement = "SnakePhaseMovement";

		private const string Snake_TailShot = "SnakeTailShot";

		private const string Snake_Tail = "SnakeTail";

		private const string Snake_Thunder = "Thunder";

		private const string Snake_ElectricTail = "SnakeElectricTail";

		private const string Snake_ElectricShot = "SnakeElectricShot";

		private const string Snake_TongleExplosion = "SnakeTongleExplosion";

		private const string Snake_Wind = "SnakeWind";

		private const string Snake_Death = "SnakeDeath";

		private const string Snake_DeathStinger = "SnakeDeathStinger";

		private int fightState = 1;
	}
}
