using System;
using System.Collections.Generic;

namespace Framework.Map
{
	public class MapData
	{
		public static List<MapData.MarkType> MarkPrivate = new List<MapData.MarkType>
		{
			MapData.MarkType.Teleport,
			MapData.MarkType.MeaCulpa,
			MapData.MarkType.PrieDieu,
			MapData.MarkType.Guilt,
			MapData.MarkType.Nacimiento,
			MapData.MarkType.Soledad,
			MapData.MarkType.Confessor,
			MapData.MarkType.FuenteFlask,
			MapData.MarkType.MiriamPortal
		};

		public string Name;

		public List<CellData> Cells = new List<CellData>();

		public Dictionary<string, SecretData> Secrets = new Dictionary<string, SecretData>();

		public Dictionary<ZoneKey, List<CellData>> CellsByZone = new Dictionary<ZoneKey, List<CellData>>();

		public Dictionary<CellKey, CellData> CellsByCellKey = new Dictionary<CellKey, CellData>();

		public Dictionary<CellKey, MapData.MarkType> Marks = new Dictionary<CellKey, MapData.MarkType>();

		public enum MarkType
		{
			Teleport,
			MeaCulpa,
			PrieDieu,
			Guilt,
			Cherub,
			Npc,
			Blue,
			Chest,
			Enemy,
			Green,
			Question,
			Red,
			Soledad,
			Nacimiento,
			Confessor,
			FuenteFlask,
			MiriamPortal
		}
	}
}
