using System;
using UnityEngine;

namespace Framework.Map
{
	[Serializable]
	public class EditorMapCellData
	{
		public EditorMapCellData()
		{
			this.Walls = new bool[4];
			this.Doors = new bool[4];
			this.CalculateWalls = new bool[4];
			this.WorldOffset = new float[4];
		}

		public EditorMapCellData(EditorMapCellData other)
		{
			this.ZoneId = new ZoneKey(other.ZoneId);
			this.Type = other.Type;
			Array.Copy(other.Walls, this.Walls, 4);
			Array.Copy(other.Doors, this.Doors, 4);
		}

		public bool NGPlus;

		public bool IgnoredForMapPercentage;

		public ZoneKey ZoneId = new ZoneKey();

		[HideInInspector]
		public EditorMapCellData.CellType Type;

		[HideInInspector]
		public bool[] Walls = new bool[4];

		[HideInInspector]
		public bool[] Doors = new bool[4];

		[HideInInspector]
		public bool[] CalculateWalls = new bool[4];

		[HideInInspector]
		public float[] WorldOffset = new float[4];

		[HideInInspector]
		public Rect CalculatedWorldBounding = Rect.zero;

		public enum CellType
		{
			Normal,
			PrieDieu,
			Teleport,
			MeaCulpa,
			Soledad,
			Nacimiento,
			Confessor,
			FuenteFlask,
			MiriamPortal
		}

		public enum CellSide
		{
			North,
			South,
			West,
			East
		}
	}
}
