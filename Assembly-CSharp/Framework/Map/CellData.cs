using System;
using UnityEngine;

namespace Framework.Map
{
	public class CellData
	{
		public CellData(CellKey cellKey, EditorMapCellData editor)
		{
			this.CellKey = cellKey;
			this.Type = editor.Type;
			this.Bounding = editor.CalculatedWorldBounding;
			this.ZoneId = new ZoneKey(editor.ZoneId);
			this.NGPlus = editor.NGPlus;
			this.IgnoredForMapPercentage = editor.IgnoredForMapPercentage;
			Array.Copy(editor.Doors, this.Doors, 4);
			Array.Copy(editor.CalculateWalls, this.Walls, 4);
			this.Revealed = false;
		}

		public EditorMapCellData.CellType Type;

		public CellKey CellKey;

		public Rect Bounding = Rect.zero;

		public ZoneKey ZoneId;

		public bool Revealed;

		public bool NGPlus;

		public bool IgnoredForMapPercentage;

		public bool[] Walls = new bool[4];

		public bool[] Doors = new bool[4];
	}
}
