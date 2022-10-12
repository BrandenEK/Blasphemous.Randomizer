using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Framework.Managers
{
	public class UIManager : GameSystem
	{
		public bool ShowGamePlayUI { get; set; }

		public bool ShowGamePlayUIForDebug { get; set; }

		public bool ConsoleShowDebugUI { get; set; }

		public bool MustShowGamePlayUI()
		{
			return this.ShowGamePlayUI && this.ShowGamePlayUIForDebug;
		}

		public override void Initialize()
		{
			this.ShowGamePlayUI = true;
			this.ShowGamePlayUIForDebug = true;
			this.ConsoleShowDebugUI = true;
			LevelManager.OnGenericsElementsLoaded += this.RefreshReferences;
			this.healthBarPrefab = (GameObject)Resources.Load("Core/EnemyHealthBar");
		}

		public void AttachHealthBarToEnemy(Enemy enemy)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.healthBarPrefab, enemy.transform);
			gameObject.GetComponent<EnemyHealthBar>().UpdateParent(enemy);
		}

		private void RefreshReferences()
		{
			Log.Trace("UI", "Refreshing UI widget references.", null);
			if (this.Glow == null)
			{
				this.Glow = UnityEngine.Object.FindObjectOfType<GlowWidget>();
			}
			if (this.Fade == null)
			{
				this.Fade = UnityEngine.Object.FindObjectOfType<FadeWidget>();
			}
			if (this.GameplayUI == null)
			{
				this.GameplayUI = UnityEngine.Object.FindObjectOfType<GameplayWidget>();
			}
			if (this.Cinematic == null)
			{
				this.Cinematic = UnityEngine.Object.FindObjectOfType<CinematicBars>();
			}
			if (this.NavigationUI == null)
			{
				this.NavigationUI = UnityEngine.Object.FindObjectOfType<NavigationWidget>();
			}
		}

		public FadeWidget Fade { get; private set; }

		public GlowWidget Glow { get; private set; }

		public CinematicBars Cinematic { get; private set; }

		public GameplayWidget GameplayUI { get; private set; }

		public NavigationWidget NavigationUI { get; private set; }

		private const string ENEMY_HEATLH_BAR_PREFAB = "Core/EnemyHealthBar";

		private const int ENEMY_HEATLH_BAR_POOL_COUNT = 30;

		private GameObject healthBarPrefab;
	}
}
