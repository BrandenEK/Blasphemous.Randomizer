using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.EditorScripts.EnemiesBalance
{
	[CreateAssetMenu(fileName = "EnemiesBalanceChart", menuName = "Blasphemous/Enemies Balance Chart")]
	public class EnemiesBalanceChart : ScriptableObject
	{
		[Button(0)]
		private void LoadBalanceChart()
		{
			this.ClearChart();
			if (this.BalanceChart == null)
			{
				return;
			}
			List<Dictionary<string, object>> balanceChartData = CSVReader.Read(this.BalanceChart);
			this.ApplyStatsBalance(balanceChartData, 0f);
		}

		private void ApplyStatsBalance(List<Dictionary<string, object>> balanceChartData, float defaultIncrementFactor = 0f)
		{
			try
			{
				for (int i = 0; i < balanceChartData.Count; i++)
				{
					EnemyBalanceItem item = new EnemyBalanceItem
					{
						Id = (string)balanceChartData[i]["Id"],
						Name = (string)balanceChartData[i]["Name"],
						LifeBase = float.Parse(balanceChartData[i]["Life Base"].ToString()),
						Strength = float.Parse(balanceChartData[i]["Strength"].ToString()),
						ContactDamage = float.Parse(balanceChartData[i]["Contact Damage"].ToString()),
						PurgePoints = int.Parse(balanceChartData[i]["Purge Points"].ToString())
					};
					this.EnemiesBalanceItems.Add(item);
				}
			}
			catch (KeyNotFoundException ex)
			{
				Debug.LogError("La clave solicitada no existe en el archivo CSV.");
			}
		}

		[Button(0)]
		private void ClearChart()
		{
			if (this.EnemiesBalanceItems != null && this.EnemiesBalanceItems.Count > 0)
			{
				this.EnemiesBalanceItems.Clear();
			}
		}

		[SerializeField]
		public TextAsset BalanceChart;

		[SerializeField]
		public List<EnemyBalanceItem> EnemiesBalanceItems;
	}
}
