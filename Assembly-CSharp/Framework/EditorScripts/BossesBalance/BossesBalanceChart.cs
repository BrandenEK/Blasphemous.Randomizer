using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.EditorScripts.BossesBalance
{
	[CreateAssetMenu(fileName = "BossesBalanceChart", menuName = "Blasphemous/Bosses Balance Chart")]
	public class BossesBalanceChart : ScriptableObject
	{
		public List<Dictionary<string, object>> BossesBalance
		{
			get
			{
				return CSVReader.Read(this.BalanceChart);
			}
		}

		[Button(0)]
		private void LoadBalanceChart()
		{
			this.ClearChart();
			if (this.BalanceChart == null)
			{
				return;
			}
			this.bossesBalance = CSVReader.Read(this.BalanceChart);
			this.LoadedBossesIds(this.bossesBalance);
		}

		private void LoadedBossesIds(List<Dictionary<string, object>> bossesBalance)
		{
			if (this.LoadedBosses == null)
			{
				this.LoadedBosses = new List<string>();
			}
			foreach (Dictionary<string, object> dictionary in bossesBalance)
			{
				this.LoadedBosses.Add(dictionary["Name"].ToString());
			}
		}

		[Button(0)]
		private void ClearChart()
		{
			if (this.LoadedBosses != null && this.LoadedBosses.Count > 0)
			{
				this.LoadedBosses.Clear();
			}
			if (this.bossesBalance != null && this.bossesBalance.Count > 0)
			{
				this.bossesBalance.Clear();
			}
		}

		[SerializeField]
		public TextAsset BalanceChart;

		[SerializeField]
		private List<Dictionary<string, object>> bossesBalance;

		[SerializeField]
		private List<string> LoadedBosses;
	}
}
