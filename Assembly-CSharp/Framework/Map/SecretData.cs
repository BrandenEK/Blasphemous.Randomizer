using System;
using System.Collections.Generic;

namespace Framework.Map
{
	public class SecretData
	{
		public string Name;

		public Dictionary<CellKey, CellData> Cells = new Dictionary<CellKey, CellData>();

		public bool Revealed;
	}
}
