using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	public class SceneAudioModifer : MonoBehaviour
	{
		private bool ShowSpawnList()
		{
			return this.checkEnemies == SceneAudioModifer.CheckEnemies.IN_SPANW_POINTS;
		}

		private void Awake()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		private void OnDestroy()
		{
			Core.Logic.EnemySpawner.OnConsumedSpanwPoint -= this.OnConsumedSpanwPoint;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		public void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			Core.Logic.EnemySpawner.OnConsumedSpanwPoint += this.OnConsumedSpanwPoint;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Penitent") && this.CheckCondition())
			{
				this.isActive = true;
				string newReverb = (!this.changeReverb) ? string.Empty : this.idReverb;
				Core.Audio.Ambient.StartModifierParams(base.name, newReverb, this.parameters);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.CompareTag("Penitent"))
			{
				this.StopModifier();
			}
		}

		private void OnConsumedSpanwPoint()
		{
			if (this.isActive && this.checkEnemies != SceneAudioModifer.CheckEnemies.DONT_CHECK && !this.CheckCondition())
			{
				this.StopModifier();
			}
		}

		private bool CheckCondition()
		{
			bool result = true;
			SceneAudioModifer.CheckEnemies checkEnemies = this.checkEnemies;
			if (checkEnemies != SceneAudioModifer.CheckEnemies.ANY_IN_SCENE)
			{
				if (checkEnemies == SceneAudioModifer.CheckEnemies.IN_SPANW_POINTS)
				{
					result = this.SpawnPoints.Any((string p) => !Core.Logic.EnemySpawner.IsSpawnerConsumed(p));
				}
			}
			else
			{
				result = Core.Logic.EnemySpawner.IsAnySpawnerLeft();
			}
			return result;
		}

		private void StopModifier()
		{
			if (this.isActive)
			{
				Core.Audio.Ambient.StopModifierParams(base.name);
				this.isActive = false;
			}
		}

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private AudioParamInitialized[] parameters = new AudioParamInitialized[0];

		[SerializeField]
		[BoxGroup("Global", true, false, 0)]
		private bool changeReverb;

		[SerializeField]
		[BoxGroup("Global", true, false, 0)]
		[EventRef]
		[ShowIf("changeReverb", true)]
		private string idReverb;

		[SerializeField]
		[BoxGroup("Checks", true, false, 0)]
		private SceneAudioModifer.CheckEnemies checkEnemies;

		[SerializeField]
		[ShowIf("ShowSpawnList", true)]
		[BoxGroup("Checks", true, false, 0)]
		private List<string> SpawnPoints = new List<string>();

		private bool isActive;

		private enum CheckEnemies
		{
			DONT_CHECK,
			ANY_IN_SCENE,
			IN_SPANW_POINTS
		}
	}
}
