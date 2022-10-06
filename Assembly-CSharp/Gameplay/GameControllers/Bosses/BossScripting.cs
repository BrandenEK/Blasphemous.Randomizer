using System;
using FMOD.Studio;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PietyMonster;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Gameplay.GameControllers.Bosses
{
	public class BossScripting : MonoBehaviour
	{
		private void Start()
		{
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(this.OnLevelLoaded);
			this.Boss = Object.FindObjectOfType<PietyMonster>();
			if (this.entity == null)
			{
				this.entity = this.Boss;
			}
			this.Boss.OnDeath += this.OnDead;
		}

		private void OnDead()
		{
			int @int = PlayerPrefs.GetInt("BOSS_TRIES", 1);
			if (!this.empoweredPiety)
			{
				Core.Metrics.CustomEvent("NORMAL_BOSS_KILLED", string.Empty, (float)@int);
			}
			else
			{
				Core.Metrics.CustomEvent("EMPOWERED_BOSS_KILLED", string.Empty, (float)@int);
			}
			Log.Trace("Metrics", "Boss fight Nº " + @int + " was successful.", null);
			PlayerPrefs.DeleteKey("BOSS_TRIES");
			this.sound.setParameterValue("Ending", 1f);
			UIController.instance.HideBossHealth();
			foreach (GameObject gameObject in this.enableObjects)
			{
				if (!(gameObject == null))
				{
					gameObject.SetActive(true);
				}
			}
		}

		private void OnLevelLoaded(Scene arg0, LoadSceneMode arg1)
		{
			if (this.sound.isValid())
			{
				this.sound.stop(1);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!this.sound.isValid() && !this.entity.Status.Dead)
			{
				PlayerPrefs.GetInt("BOSS_TRIES", 1);
				int @int = PlayerPrefs.GetInt("BOSS_TRIES");
				Log.Trace("Metrics", "Boss fight Nº" + @int + " started.", null);
				this.sound = Core.Audio.CreateCatalogEvent(this.id, default(Vector3));
				this.sound.start();
				UIController.instance.ShowBossHealth(this.entity);
				if (this.entity is Enemy)
				{
					Enemy enemy = (Enemy)this.entity;
					enemy.UseHealthBar = false;
				}
				Core.Logic.CameraManager.CameraPlayerOffset.XOffset = 1f;
			}
		}

		private void Update()
		{
			if (!this.sound.isValid())
			{
				return;
			}
			switch (this.Boss.CurrentBossStatus)
			{
			case PietyMonster.BossStatus.First:
				this.sound.setParameterValue("Intensity", 1f);
				break;
			case PietyMonster.BossStatus.Second:
				this.sound.setParameterValue("Intensity", 3f);
				break;
			case PietyMonster.BossStatus.Third:
				this.sound.setParameterValue("Intensity", 4f);
				this.sound.setParameterValue("State2", 1f);
				break;
			case PietyMonster.BossStatus.Forth:
				this.sound.setParameterValue("Intensity", 5f);
				break;
			case PietyMonster.BossStatus.Fifth:
				this.sound.setParameterValue("Intensity", 6f);
				this.sound.setParameterValue("State3", 1f);
				break;
			case PietyMonster.BossStatus.Sixth:
				this.sound.setParameterValue("Intensity", 8f);
				this.sound.setParameterValue("State4", 1f);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private const string INTENSITY_PARAM = "Intensity";

		private const string STATE_PARAM2 = "State2";

		private const string STATE_PARAM3 = "State3";

		private const string STATE_PARAM4 = "State4";

		private const string STATE_PARAM5 = "State5";

		private const string STATE_PARAM6 = "State6";

		private const string ENDING_PARAM = "Ending";

		[SerializeField]
		private string id;

		[SerializeField]
		private Entity entity;

		[SerializeField]
		private bool empoweredPiety;

		[SerializeField]
		private GameObject[] enableObjects;

		private PietyMonster Boss;

		private EventInstance sound;
	}
}
