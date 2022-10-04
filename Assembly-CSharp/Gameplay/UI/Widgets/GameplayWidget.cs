using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Penitences;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Others.UIGameLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class GameplayWidget : UIWidget
	{
		private void Awake()
		{
			this.canvas = base.GetComponent<CanvasGroup>();
			PenitenceManager.OnPenitenceChanged += this.OnPenitenceChanged;
			this.OnPenitenceChanged(Core.PenitenceManager.GetCurrentPenitence(), null);
		}

		private void OnDestroy()
		{
			PenitenceManager.OnPenitenceChanged -= this.OnPenitenceChanged;
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoadDemake;
		}

		public void RestoreDefaultPanelsStatus()
		{
			this.miriamTimer.Hide();
			this.bossRushTimer.Hide();
			this.purgePointsDemake.gameObject.SetActive(true);
			this.ShowPurgePoints();
		}

		private void OnPenitenceChanged(IPenitence current, List<IPenitence> completed)
		{
			bool isPE02Active = current is PenitencePE02;
			this.normalHealthGameObjects.ForEach(delegate(GameObject x)
			{
				x.SetActive(!isPE02Active);
			});
			this.pe02HealthGameObjects.ForEach(delegate(GameObject x)
			{
				x.SetActive(isPE02Active);
			});
			this.normalPlayerHealth.enabled = !isPE02Active;
			this.pe02PlayerHealth.enabled = isPE02Active;
			if (isPE02Active)
			{
				this.pe02PlayerHealth.ForceUpdate();
			}
			Sprite sprite = null;
			if (current != null)
			{
				foreach (SelectSaveSlots.PenitenceData penitenceData in this.PenitencesConfig)
				{
					if (penitenceData.id.ToUpper() == current.Id.ToUpper())
					{
						sprite = penitenceData.InProgress;
					}
				}
			}
			this.CurrentPenitence.enabled = (sprite != null);
			this.CurrentPenitence.sprite = sprite;
		}

		private void OnBeforeLevelLoadDemake(Level oldLevel, Level newLevel)
		{
			if (newLevel.LevelName.StartsWith("D25"))
			{
				this.demakeUiIsActive = false;
			}
			else
			{
				this.demakeUiIsActive = true;
			}
		}

		private void Update()
		{
			LogicManager logic = Core.Logic;
			UIManager ui = Core.UI;
			if (logic == null || ui == null)
			{
				return;
			}
			bool flag = !logic.IsMenuScene() && !logic.IsAttrackScene() && ui.MustShowGamePlayUI();
			this.canvas.alpha = ((!flag) ? 0f : 1f);
			if (flag)
			{
				for (int i = 0; i < 4; i++)
				{
					this.keys[i].SetActive(Core.InventoryManager.CheckBossKey(i));
				}
			}
			bool enableDemakeUi = Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			this.UpdateDemakeUI(enableDemakeUi);
		}

		private void UpdateDemakeUI(bool enableDemakeUi)
		{
			if (this.demakeUiIsActive && !enableDemakeUi)
			{
				this.demakeUiIsActive = false;
				this.purgePointsDemake.gameObject.SetActive(false);
				this.purgePoints.gameObject.SetActive(true);
				this.deactivateInDemakeGameObjects.ForEach(delegate(GameObject x)
				{
					x.SetActive(true);
				});
				IPenitence currentPenitence = Core.PenitenceManager.GetCurrentPenitence();
				if (currentPenitence != null && currentPenitence is PenitencePE02)
				{
					this.normalHealthGameObjects.ForEach(delegate(GameObject x)
					{
						x.SetActive(false);
					});
					this.pe02HealthGameObjects.ForEach(delegate(GameObject x)
					{
						x.SetActive(true);
					});
					this.demakeHealthGameObjects.ForEach(delegate(GameObject x)
					{
						x.SetActive(false);
					});
					this.normalPlayerHealth.enabled = false;
					this.pe02PlayerHealth.enabled = true;
					this.demakePlayerHealth.enabled = false;
				}
				else
				{
					this.normalHealthGameObjects.ForEach(delegate(GameObject x)
					{
						x.SetActive(true);
					});
					this.pe02HealthGameObjects.ForEach(delegate(GameObject x)
					{
						x.SetActive(false);
					});
					this.demakeHealthGameObjects.ForEach(delegate(GameObject x)
					{
						x.SetActive(false);
					});
					this.normalPlayerHealth.enabled = true;
					this.pe02PlayerHealth.enabled = false;
					this.demakePlayerHealth.enabled = false;
				}
			}
			else if (!this.demakeUiIsActive && enableDemakeUi)
			{
				this.demakeUiIsActive = true;
				this.deactivateInDemakeGameObjects.ForEach(delegate(GameObject x)
				{
					x.SetActive(false);
				});
				this.normalHealthGameObjects.ForEach(delegate(GameObject x)
				{
					x.SetActive(false);
				});
				this.pe02HealthGameObjects.ForEach(delegate(GameObject x)
				{
					x.SetActive(false);
				});
				this.demakeHealthGameObjects.ForEach(delegate(GameObject x)
				{
					x.SetActive(true);
				});
				this.normalPlayerHealth.enabled = false;
				this.pe02PlayerHealth.enabled = false;
				this.demakePlayerHealth.enabled = true;
				this.purgePointsDemake.gameObject.SetActive(true);
				this.purgePoints.gameObject.SetActive(false);
				LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoadDemake;
				LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoadDemake;
			}
		}

		public void UpdatePurgePoints()
		{
			if (this.purgePoints.gameObject.activeInHierarchy)
			{
				this.purgePoints.RefreshPoints(true);
			}
			else if (this.purgePointsDemake.gameObject.activeInHierarchy)
			{
				this.purgePointsDemake.RefreshPoints(true);
			}
		}

		public void ShowPurgePoints()
		{
			this.purgePoints.gameObject.SetActive(true);
		}

		public void ShowLeftPart()
		{
			this.leftPartRoot.SetActive(true);
		}

		public void HideLeftPart()
		{
			this.leftPartRoot.SetActive(false);
		}

		public void ShowRightPart()
		{
			this.rightPartRoot.SetActive(true);
		}

		public void HideRightPart()
		{
			this.rightPartRoot.SetActive(false);
		}

		public void HidePurgePoints()
		{
			this.purgePoints.gameObject.SetActive(false);
		}

		public void ShowBossRushTimer()
		{
			this.bossRushTimer.Show();
		}

		public void HideBossRushTimer()
		{
			this.bossRushTimer.Hide();
		}

		public void UpdateGuiltLevel(bool whenDead)
		{
			if (this.purgePoints.gameObject.activeInHierarchy)
			{
				this.purgePoints.RefreshGuilt(whenDead);
			}
			if (this.fervour.gameObject.activeInHierarchy)
			{
				this.fervour.RefreshGuilt(whenDead);
			}
		}

		public void NotEnoughFervour()
		{
			this.fervour.NotEnoughFervour();
		}

		public void StartMiriamTimer()
		{
			this.miriamTimer.StartTimer();
		}

		public void StopMiriamTimer()
		{
			this.miriamTimer.StopTimer(true);
		}

		public void SetMiriamTimerTargetTime(float targetTime)
		{
			this.miriamTimer.SetTargetTime(targetTime);
		}

		public void ShowMiriamTimer()
		{
			this.miriamTimer.Show();
		}

		public void HideMiriamTimer()
		{
			this.miriamTimer.Hide();
		}

		[SerializeField]
		private GameObject leftPartRoot;

		[SerializeField]
		private GameObject rightPartRoot;

		[SerializeField]
		private GameObject[] keys = new GameObject[4];

		[SerializeField]
		private PlayerPurgePoints purgePoints;

		[SerializeField]
		private PlayerPurgePoints purgePointsDemake;

		[SerializeField]
		private PlayerFervour fervour;

		[SerializeField]
		private BossRushTimer bossRushTimer;

		[SerializeField]
		private MiriamTimer miriamTimer;

		[SerializeField]
		private List<GameObject> normalHealthGameObjects;

		[SerializeField]
		private List<GameObject> pe02HealthGameObjects;

		[SerializeField]
		private List<GameObject> demakeHealthGameObjects;

		[SerializeField]
		private List<GameObject> deactivateInDemakeGameObjects;

		[SerializeField]
		private PlayerHealth normalPlayerHealth;

		[SerializeField]
		private PlayerHealthPE02 pe02PlayerHealth;

		[SerializeField]
		private PlayerHealthDemake demakePlayerHealth;

		[SerializeField]
		private Image CurrentPenitence;

		[SerializeField]
		[BoxGroup("Penitence", true, false, 0)]
		private List<SelectSaveSlots.PenitenceData> PenitencesConfig;

		private CanvasGroup canvas;

		private bool demakeUiIsActive;
	}
}
