using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Sirenix.Utilities;
using UnityAnalyticsHeatmap;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace Framework.Managers
{
	public class MetricManager : GameSystem
	{
		public override void Start()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			this.InputEvent();
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.SceneTimeCount();
		}

		public override void Update()
		{
			this.SceneTimeCounterUpdate();
		}

		public void CustomEvent(string eventId, string name = "", float amount = -1f)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (!name.IsNullOrWhitespace())
			{
				dictionary.Add("NAME", name);
			}
			if (amount > -1f)
			{
				dictionary.Add("AMOUNT", amount);
			}
			dictionary.Add("SCENE", SceneManager.GetActiveScene().name);
			Analytics.CustomEvent(eventId, dictionary);
		}

		public void HeatmapEvent(string eventId, Vector2 position)
		{
			UnityAnalyticsHeatmap.HeatmapEvent.Send(eventId, position, null);
		}

		private void InputEvent()
		{
		}

		private void SceneTimeCount()
		{
			if (this.trackedScene == null)
			{
				this.trackedScene = SceneManager.GetActiveScene().name;
				return;
			}
			if (this.trackedScene == SceneManager.GetActiveScene().name)
			{
				return;
			}
			this.CustomEvent("TIME_IN_SCENE", string.Empty, this.timeInScene);
			this.timeInScene = 0f;
			this.trackedScene = SceneManager.GetActiveScene().name;
		}

		private void SceneTimeCounterUpdate()
		{
			this.timeInScene += Time.deltaTime;
			if (this.trackedScene != SceneManager.GetActiveScene().name)
			{
				this.SceneTimeCount();
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		private float timeInScene;

		private string trackedScene;
	}
}
