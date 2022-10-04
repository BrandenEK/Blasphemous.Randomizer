using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Recolor
{
	public class ColorPaletteDictionary : ScriptableObject
	{
		public Sprite GetPalette(string id)
		{
			PalettesById palettesById = this.PalettesById.Find((PalettesById x) => x.id == id);
			if (palettesById.id == string.Empty)
			{
				Debug.LogError(string.Format("Palette with id {0} not found", id));
			}
			return palettesById.paletteTex;
		}

		public Sprite GetPreview(string id)
		{
			PalettesById palettesById = this.PalettesById.Find((PalettesById x) => x.id == id);
			if (palettesById.id == string.Empty)
			{
				Debug.LogError(string.Format("Palette with id {0} not found", id));
			}
			return palettesById.palettePreview;
		}

		public List<string> GetAllIds()
		{
			List<string> list = new List<string>();
			foreach (PalettesById palettesById in this.PalettesById)
			{
				list.Add(palettesById.id);
			}
			return list;
		}

		public List<Sprite> GetAllPalettes()
		{
			List<Sprite> list = new List<Sprite>();
			foreach (PalettesById palettesById in this.PalettesById)
			{
				list.Add(palettesById.paletteTex);
			}
			return list;
		}

		public List<PalettesById> PalettesById;
	}
}
