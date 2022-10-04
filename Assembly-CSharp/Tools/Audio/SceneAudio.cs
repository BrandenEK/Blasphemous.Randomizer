using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	[DefaultExecutionOrder(1)]
	public class SceneAudio : MonoBehaviour
	{
		private void Awake()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			Core.Audio.Ambient.SetSceneParams(this.trackIdentifier, this.idReverb, this.globalParameters, newLevel.LevelName);
			Core.Audio.Ambient.SetAmbientParams(this.ambientParameters, this.StartTime, this.EndTime);
		}

		public void RestartSceneAudio()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			Core.Audio.Ambient.StopCurrent();
			Core.Audio.Ambient.SetSceneParams(this.trackIdentifier, this.idReverb, this.globalParameters, Core.LevelManager.currentLevel.LevelName);
			Core.Audio.Ambient.SetAmbientParams(this.ambientParameters, this.StartTime, this.EndTime);
		}

		[SerializeField]
		[BoxGroup("Global", true, false, 0)]
		private AudioParam[] globalParameters = new AudioParam[0];

		[SerializeField]
		[BoxGroup("Global", true, false, 0)]
		[EventRef]
		private string trackIdentifier;

		[SerializeField]
		[BoxGroup("Global", true, false, 0)]
		[EventRef]
		private string idReverb;

		[SerializeField]
		[BoxGroup("Ambient", true, false, 0)]
		private AudioParamInitialized[] ambientParameters = new AudioParamInitialized[0];

		[SerializeField]
		[BoxGroup("Ambient", true, false, 0)]
		protected float StartTime = 10f;

		[SerializeField]
		[BoxGroup("Ambient", true, false, 0)]
		protected float EndTime = 300f;
	}
}
