using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Tools.DataContainer
{
	[CreateAssetMenu(fileName = "MapData", menuName = "Blasphemous/Map")]
	public class MapData : SerializedScriptableObject
	{
		[OdinSerialize]
		public Dictionary<string, MapData.MapItem> data;

		[Serializable]
		public class MapItem
		{
			public string GetKey()
			{
				return this.domain + "_" + this.zone;
			}

			public string mapImage = string.Empty;

			public Vector3 position = Vector3.zero;

			public string domain = string.Empty;

			public string zone = string.Empty;

			public int height;

			public int width;

			public float cameraSize;

			[OdinSerialize]
			public List<Bounds> cells = new List<Bounds>();
		}
	}
}
