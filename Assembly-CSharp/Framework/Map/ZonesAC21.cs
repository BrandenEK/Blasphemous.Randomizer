using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Map
{
	[CreateAssetMenu(menuName = "Blasphemous/Map Zones config")]
	public class ZonesAC21 : SerializedScriptableObject
	{
		public bool AllowZoneForAc21(ZoneKey zone)
		{
			return this.AllowZoneForAc21(zone.District, zone.Zone);
		}

		public bool AllowZoneForAc21(string district, string zone)
		{
			bool result = false;
			string key = district.ToUpper() + zone.ToUpper();
			if (this.ZonesConfig.ContainsKey(key))
			{
				result = this.ZonesConfig[key];
			}
			return result;
		}

		public Dictionary<string, bool> ZonesConfig = new Dictionary<string, bool>();
	}
}
