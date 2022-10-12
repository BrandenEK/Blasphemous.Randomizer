using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Managers;
using UnityEngine;

namespace Tools.Audio
{
	public class AmbientMusicSettings
	{
		public AmbientMusicSettings()
		{
			this.audioInstance = default(EventInstance);
			this.trackIdentifier = string.Empty;
			this.reverbInstance = default(EventInstance);
			this.reverbIdentifier = string.Empty;
			this.reverbIdentifierPrevious = string.Empty;
			this.volume = 1f;
			this.ambientRunning = false;
			this.ambientCurrentTime = 0f;
			this.ambientStartTime = 0f;
			this.ambientEndTime = 0f;
			this.ambientNextStatus = AmbientMusicSettings.AmbientStatus.Start;
			this.sceneParams = new Dictionary<string, float>();
			this.ambientParams = new Dictionary<string, AmbientMusicSettings.AudioParamInitializedClass>();
			this.modifierParams = new Dictionary<string, Dictionary<string, AmbientMusicSettings.AudioParamInitializedClass>>();
			this.areaModifiers = new Dictionary<string, float>();
		}

		public EventInstance AudioInstance
		{
			get
			{
				return this.audioInstance;
			}
		}

		public void SetSceneParams(string idTrack, string idReverb, AudioParam[] globalParameters, string newLevelName = "")
		{
			Debug.Log(string.Concat(new string[]
			{
				"** NEW Scene Audio: PlayTrack ",
				idTrack,
				" reverb ",
				idReverb,
				",volume ",
				this.volume.ToString()
			}));
			if (idTrack != this.trackIdentifier)
			{
				if (this.audioInstance.isValid() && !newLevelName.Equals("D07Z01S04"))
				{
					this.StopCurrent();
					this.audioInstance = default(EventInstance);
				}
				this.trackIdentifier = idTrack;
				if (idTrack != string.Empty)
				{
					this.audioInstance = Core.Audio.CreateEvent(idTrack, default(Vector3));
					this.audioInstance.start();
					this.SetAudioInstanceVolume();
				}
			}
			this.SetReverb(idReverb);
			this.reverbIdentifierPrevious = string.Empty;
			for (int i = 0; i < globalParameters.Length; i++)
			{
				this.audioInstance.setParameterValue(globalParameters[i].name, globalParameters[i].targetValue);
				this.sceneParams[globalParameters[i].name] = globalParameters[i].targetValue;
			}
		}

		public void SetSceneParam(AudioParam parameter)
		{
			if (!this.audioInstance.isValid())
			{
				return;
			}
			this.audioInstance.setParameterValue(parameter.name, parameter.targetValue);
			this.sceneParams[parameter.name] = parameter.targetValue;
		}

		public void SetSceneParam(string key, float value)
		{
			this.SetSceneParam(new AudioParam
			{
				name = key,
				targetValue = value
			});
		}

		public void SetAmbientParams(AudioParamInitialized[] parameters, float startTime, float endTime)
		{
			this.ambientRunning = (parameters.Length > 0);
			if (this.ambientRunning)
			{
				this.ambientStartTime = startTime;
				this.ambientEndTime = endTime;
				foreach (AudioParamInitialized audioParamInitialized in parameters)
				{
					float currentValue = 0f;
					if (this.ambientParams.ContainsKey(audioParamInitialized.name))
					{
						currentValue = this.ambientParams[audioParamInitialized.name].currentValue;
					}
					this.ambientParams[audioParamInitialized.name] = new AmbientMusicSettings.AudioParamInitializedClass(audioParamInitialized.enterValue, audioParamInitialized.exitValue);
					this.ambientParams[audioParamInitialized.name].currentValue = currentValue;
				}
			}
		}

		public void AddAreaModifier(string name, float value)
		{
			if (value == 0f && this.areaModifiers.ContainsKey(name))
			{
				this.areaModifiers.Remove(name);
			}
			else
			{
				this.areaModifiers[name] = value;
			}
		}

		public void StartModifierParams(string name, string newReverb, AudioParamInitialized[] parameters)
		{
			Dictionary<string, AmbientMusicSettings.AudioParamInitializedClass> dictionary = new Dictionary<string, AmbientMusicSettings.AudioParamInitializedClass>();
			foreach (AudioParamInitialized audioParamInitialized in parameters)
			{
				dictionary[audioParamInitialized.name] = new AmbientMusicSettings.AudioParamInitializedClass(audioParamInitialized.enterValue, audioParamInitialized.exitValue);
				dictionary[audioParamInitialized.name].currentValue = audioParamInitialized.enterValue;
				this.audioInstance.setParameterValue(audioParamInitialized.name, audioParamInitialized.enterValue);
			}
			this.modifierParams[name] = dictionary;
			if (newReverb != string.Empty && newReverb != this.reverbIdentifier)
			{
				this.reverbIdentifierPrevious = this.reverbIdentifier;
				this.SetReverb(newReverb);
			}
		}

		public void StopModifierParams(string name)
		{
			if (!this.modifierParams.ContainsKey(name))
			{
				return;
			}
			foreach (KeyValuePair<string, AmbientMusicSettings.AudioParamInitializedClass> keyValuePair in this.modifierParams[name])
			{
				this.audioInstance.setParameterValue(keyValuePair.Key, keyValuePair.Value.exitValue);
			}
			this.modifierParams.Remove(name);
			if (this.reverbIdentifierPrevious != string.Empty)
			{
				this.SetReverb(this.reverbIdentifierPrevious);
				this.reverbIdentifierPrevious = string.Empty;
			}
		}

		public void StopCurrent()
		{
			this.StopReverb();
			if (!this.audioInstance.isValid())
			{
				return;
			}
			Debug.Log("** Fadeout last " + this.trackIdentifier);
			this.audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.audioInstance.release();
			this.audioInstance = default(EventInstance);
			this.trackIdentifier = string.Empty;
			this.reverbIdentifierPrevious = string.Empty;
			this.modifierParams.Clear();
			this.ambientParams.Clear();
			this.sceneParams.Clear();
			this.areaModifiers.Clear();
			this.ambientRunning = false;
			this.ambientCurrentTime = 0f;
			this.ambientNextStatus = AmbientMusicSettings.AmbientStatus.Start;
		}

		private void StopReverb()
		{
			if (!this.reverbInstance.isValid())
			{
				return;
			}
			this.reverbInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.reverbInstance.release();
			this.reverbInstance = default(EventInstance);
			this.reverbIdentifier = string.Empty;
		}

		public float Volume
		{
			get
			{
				if (this.audioInstance.isValid())
				{
					float num;
					this.audioInstance.getVolume(out this.volume, out num);
				}
				return this.volume;
			}
			set
			{
				this.volume = Mathf.Clamp01(value);
				if (this.audioInstance.isValid())
				{
					this.audioInstance.setVolume(this.volume);
				}
			}
		}

		public float GetParameterValue(string param)
		{
			float result = -1f;
			float num = -1f;
			if (this.audioInstance.isValid())
			{
				this.audioInstance.getParameterValue(param, out result, out num);
			}
			return result;
		}

		public void SetParameter(string param, float value)
		{
			if (this.audioInstance.isValid())
			{
				this.audioInstance.setParameterValue(param, value);
			}
		}

		public void Update()
		{
			if (!this.ambientRunning || !this.audioInstance.isValid())
			{
				return;
			}
			this.ambientCurrentTime += Time.deltaTime;
			if (this.ambientCurrentTime >= this.ambientStartTime && this.ambientNextStatus == AmbientMusicSettings.AmbientStatus.Start)
			{
				this.ambientNextStatus = AmbientMusicSettings.AmbientStatus.End;
				this.SetAmbientParameters(true);
			}
			if (this.ambientCurrentTime >= this.ambientEndTime && this.ambientNextStatus == AmbientMusicSettings.AmbientStatus.End)
			{
				this.ambientNextStatus = AmbientMusicSettings.AmbientStatus.Start;
				this.ambientCurrentTime = 0f;
				this.SetAmbientParameters(false);
			}
		}

		public void OnGUI()
		{
			this.posYGUI = 10f;
			this.DrawTextLine("Backgorund Music -------------------------------------");
			this.DrawTextLine("Sound:" + this.trackIdentifier);
			this.DrawTextLine("Reverb:" + this.reverbIdentifier);
			this.DrawTextLine("***** Params: Gloabal");
			foreach (KeyValuePair<string, float> keyValuePair in this.sceneParams)
			{
				this.DrawTextLine(keyValuePair.Key + ": " + keyValuePair.Value.ToString());
			}
			this.DrawTextLine("Ambient ------------");
			this.DrawTextLine("Time:" + this.ambientCurrentTime.ToString());
			this.DrawTextLine("Start:" + this.ambientStartTime.ToString());
			this.DrawTextLine("End:" + this.ambientEndTime.ToString());
			this.DrawTextLine("***** Params: AmbientMusic->" + this.ambientRunning.ToString());
			foreach (KeyValuePair<string, AmbientMusicSettings.AudioParamInitializedClass> keyValuePair2 in this.ambientParams)
			{
				this.DrawTextLine(keyValuePair2.Key + ": " + keyValuePair2.Value.currentValue.ToString());
			}
			this.DrawTextLine("Modifiers ------------");
			foreach (KeyValuePair<string, Dictionary<string, AmbientMusicSettings.AudioParamInitializedClass>> keyValuePair3 in this.modifierParams)
			{
				this.DrawTextLine("*** Modifier: " + keyValuePair3.Key);
				foreach (KeyValuePair<string, AmbientMusicSettings.AudioParamInitializedClass> keyValuePair4 in keyValuePair3.Value)
				{
					this.DrawTextLine(keyValuePair4.Key + ": " + keyValuePair4.Value.currentValue.ToString());
				}
			}
			this.DrawTextLine("Area ------------");
			foreach (KeyValuePair<string, float> keyValuePair5 in this.areaModifiers)
			{
				this.DrawTextLine("* " + keyValuePair5.Key + ": " + keyValuePair5.Value.ToString());
			}
			if (this.audioInstance.isValid())
			{
				this.DrawTextLine("***** ALL Instance Params");
				foreach (KeyValuePair<string, AmbientMusicSettings.AudioParamInitializedClass> keyValuePair6 in this.ambientParams)
				{
					float num = -1f;
					int num2 = 0;
					this.audioInstance.getParameterCount(out num2);
					for (int i = 0; i < num2; i++)
					{
						ParameterInstance parameterInstance = default(ParameterInstance);
						this.audioInstance.getParameterByIndex(i, out parameterInstance);
						PARAMETER_DESCRIPTION parameter_DESCRIPTION;
						parameterInstance.getDescription(out parameter_DESCRIPTION);
						parameterInstance.getValue(out num);
						this.DrawTextLine(parameter_DESCRIPTION.name + ": " + num.ToString());
					}
				}
			}
		}

		private void SetReverb(string idReverb)
		{
			if (idReverb != this.reverbIdentifier)
			{
				if (this.reverbInstance.isValid())
				{
					this.StopReverb();
				}
				this.reverbIdentifier = idReverb;
				if (idReverb != string.Empty)
				{
					this.reverbInstance = Core.Audio.CreateEvent(idReverb, default(Vector3));
					this.reverbInstance.start();
				}
				else
				{
					this.reverbInstance = default(EventInstance);
				}
			}
		}

		private void DrawTextLine(string text)
		{
			GUI.Label(new Rect(10f, this.posYGUI, 500f, this.posYGUI + 10f), text);
			this.posYGUI += 13f;
		}

		private void SetAudioInstanceVolume()
		{
			this.audioInstance.setVolume(this.volume);
			this.audioInstance.setParameterValue("VOLUME", 1f);
			this.audioInstance.setParameterValue("ACTIVE", 1f);
		}

		private void SetAmbientParameters(bool enableParam)
		{
			foreach (KeyValuePair<string, AmbientMusicSettings.AudioParamInitializedClass> keyValuePair in this.ambientParams)
			{
				keyValuePair.Value.currentValue = ((!enableParam) ? keyValuePair.Value.exitValue : keyValuePair.Value.enterValue);
				this.audioInstance.setParameterValue(keyValuePair.Key, keyValuePair.Value.currentValue);
			}
		}

		private EventInstance audioInstance;

		private EventInstance reverbInstance;

		private string trackIdentifier;

		private string reverbIdentifier;

		private string reverbIdentifierPrevious;

		private float volume;

		private Dictionary<string, float> sceneParams;

		private Dictionary<string, Dictionary<string, AmbientMusicSettings.AudioParamInitializedClass>> modifierParams;

		private Dictionary<string, AmbientMusicSettings.AudioParamInitializedClass> ambientParams;

		private bool ambientRunning;

		private float ambientCurrentTime;

		private float ambientStartTime;

		private float ambientEndTime;

		private AmbientMusicSettings.AmbientStatus ambientNextStatus;

		private Dictionary<string, float> areaModifiers;

		private float posYGUI;

		private class AudioParamInitializedClass
		{
			public AudioParamInitializedClass(float enter, float exit)
			{
				this.enterValue = enter;
				this.exitValue = exit;
				this.currentValue = 0f;
			}

			public float enterValue;

			public float exitValue;

			public float currentValue;
		}

		private enum AmbientStatus
		{
			Start,
			End
		}
	}
}
