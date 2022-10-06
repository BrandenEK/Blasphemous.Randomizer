using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Framework.Achievements
{
	[Serializable]
	public class AC39Enemies : SerializedScriptableObject
	{
		[Button(0)]
		public void ResetDefaultListOfEnemies()
		{
			this.EnemiesList.Clear();
			foreach (string id in this.ENEMIES_IDS_FOR_AC39)
			{
				EnemyIdAndName item = default(EnemyIdAndName);
				item.id = id;
				this.EnemiesList.Add(item);
			}
		}

		public List<EnemyIdAndName> EnemiesList = new List<EnemyIdAndName>();

		private readonly List<string> ENEMIES_IDS_FOR_AC39 = new List<string>
		{
			"EN01",
			"EN02",
			"EN03",
			"EN04",
			"EN05",
			"EN07",
			"EN08",
			"EN09",
			"EN10",
			"EN11",
			"EN12",
			"EN13",
			"EN14",
			"EN15",
			"EN16",
			"EN17",
			"EN18",
			"EN20",
			"EN21",
			"EN22",
			"EN23",
			"EN24",
			"EN26",
			"EN27",
			"EN28",
			"EN29",
			"EN31",
			"EN32",
			"EN33",
			"EN34",
			"EV01",
			"EV02",
			"EV03",
			"EV05",
			"EV08",
			"EV10",
			"EV11",
			"EV12",
			"EV13",
			"EV14",
			"EV15",
			"EV17",
			"EV18",
			"EV19",
			"EV21",
			"EV22",
			"EV23",
			"EV24",
			"EV26",
			"EV27",
			"EV29"
		};
	}
}
