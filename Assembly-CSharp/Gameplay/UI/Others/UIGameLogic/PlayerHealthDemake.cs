using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class PlayerHealthDemake : MonoBehaviour
	{
		private static int CurrentStocks
		{
			get
			{
				float num = Core.Logic.Penitent.Stats.Life.Current / 30f;
				return Mathf.CeilToInt(num);
			}
		}

		private static int MaxStocks
		{
			get
			{
				return Mathf.CeilToInt(Core.Logic.Penitent.Stats.Life.CurrentMax / 30f);
			}
		}

		private int HealthAsStocks
		{
			get
			{
				float num = this.penitent.Stats.Life.Current;
				return Mathf.CeilToInt(num / 30f);
			}
		}

		public static float StocksDamage
		{
			get
			{
				float num = Core.Logic.Penitent.Stats.Life.Current % 30f;
				return (Mathf.Abs(num) >= Mathf.Epsilon) ? num : 30f;
			}
		}

		public static float StocksHeal
		{
			get
			{
				int num = PlayerHealthDemake.CurrentStocks + 1;
				float num2 = Mathf.Min((float)num * 30f, Core.Logic.Penitent.Stats.Life.CurrentMax);
				return num2 - Core.Logic.Penitent.Stats.Life.Current;
			}
		}

		private void Awake()
		{
			LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoad;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			base.enabled = false;
		}

		private void OnDestroy()
		{
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoad;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		private void OnBeforeLevelLoad(Level oldLevel, Level newLevel)
		{
			base.enabled = false;
			this.penitent = null;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (Core.ready && Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.MENU))
			{
				this.lastMaxStocks = -1;
				this.prevFrameFillAmount = -1f;
				this.penitent = null;
			}
			else
			{
				base.enabled = true;
				this.penitent = Core.Logic.Penitent;
				PlayerHealthDemake.FillAmount = 0f;
			}
		}

		private void Update()
		{
			if (this.penitent == null)
			{
				return;
			}
			this.UpdateDisplayedStocks();
			this.UpdateFilledStocks();
			this.UpdateLinks();
			this.UpdateLastStockFillAmount();
			this.prevFrameFillAmount = PlayerHealthDemake.FillAmount;
		}

		public void ForceUpdate()
		{
			this.lastMaxStocks = -1;
			this.prevFrameFillAmount = -1f;
			this.penitent = Core.Logic.Penitent;
		}

		private void UpdateDisplayedStocks()
		{
			int maxStocks = PlayerHealthDemake.MaxStocks;
			if (maxStocks != this.lastMaxStocks)
			{
				this.lastMaxStocks = maxStocks;
				this.stocks.ForEach(delegate(GameObject x)
				{
					x.SetActive(false);
				});
				this.links.ForEach(delegate(GameObject x)
				{
					x.SetActive(false);
				});
				for (int i = 0; i < maxStocks; i++)
				{
					this.stocks[i].SetActive(true);
					if (i > 0)
					{
						this.links[i - 1].SetActive(true);
					}
				}
			}
		}

		private void UpdateFilledStocks()
		{
			int currentlyFilledStocks = this.GetCurrentlyFilledStocks();
			if (currentlyFilledStocks != this.HealthAsStocks)
			{
				for (int i = 0; i < this.stocks.Count; i++)
				{
					GameObject gameObject = this.stocks[i];
					Transform child = gameObject.transform.GetChild(0);
					Image component = child.GetComponent<Image>();
					component.fillAmount = 1f;
					component.color = Color.white;
					child.gameObject.SetActive(i < this.HealthAsStocks);
				}
			}
		}

		private void UpdateLinks()
		{
			for (int i = 1; i < this.links.Count; i++)
			{
				float alpha = (i - 1 >= this.HealthAsStocks) ? 0f : 1f;
				this.links[i - 1].GetComponent<Image>().canvasRenderer.SetAlpha(alpha);
			}
		}

		private void UpdateLastStockFillAmount()
		{
			if (this.prevFrameFillAmount == PlayerHealthDemake.FillAmount)
			{
				return;
			}
			this.prevFrameFillAmount = PlayerHealthDemake.FillAmount;
			if (PlayerHealthDemake.FillAmount >= 1f)
			{
				Image component = this.stocks[this.HealthAsStocks - 1].transform.GetChild(0).GetComponent<Image>();
				component.color = Color.white;
				component.fillAmount = 1f;
				PlayerHealthDemake.FillAmount = 0f;
			}
			else if (PlayerHealthDemake.FillAmount > 0f && PlayerHealthDemake.FillAmount < 1f)
			{
				this.stocks[this.HealthAsStocks].transform.GetChild(0).gameObject.SetActive(true);
				Image component2 = this.stocks[this.HealthAsStocks].transform.GetChild(0).GetComponent<Image>();
				component2.color = this.fillingColor;
				component2.fillAmount = PlayerHealthDemake.FillAmount;
			}
			else
			{
				this.stocks[this.HealthAsStocks].transform.GetChild(0).gameObject.SetActive(false);
			}
		}

		private int GetCurrentlyFilledStocks()
		{
			return this.stocks.FindAll((GameObject x) => x.transform.GetChild(0).gameObject.activeSelf && x.transform.GetChild(0).GetComponent<Image>().fillAmount == 1f).Count;
		}

		public static float FillAmount;

		[SerializeField]
		private Color fillingColor;

		[SerializeField]
		private List<GameObject> stocks;

		[SerializeField]
		private List<GameObject> links;

		private int lastMaxStocks = -1;

		private float prevFrameFillAmount = -1f;

		private Penitent penitent;
	}
}
