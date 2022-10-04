using System;
using System.Collections.Generic;
using Sirenix.Serialization;

namespace Framework.Map
{
	[Serializable]
	public class EditorMapCellGrid
	{
		public EditorMapCellData this[CellKey index]
		{
			get
			{
				EditorMapCellData result = null;
				if (index != null)
				{
					this.CellsDict.TryGetValue(index, out result);
				}
				return result;
			}
			set
			{
				this.CellsDict[index] = value;
			}
		}

		[OdinSerialize]
		public Dictionary<CellKey, EditorMapCellData> CellsDict = new Dictionary<CellKey, EditorMapCellData>();
	}
}
