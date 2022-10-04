using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Recolor
{
	[Serializable]
	public struct PalettesById
	{
		[BoxGroup("Palette data", true, false, 0)]
		public string id;

		[BoxGroup("Palette data", true, false, 0)]
		[LabelWidth(100f)]
		public Sprite paletteTex;

		[BoxGroup("Palette data", true, false, 0)]
		[PreviewField(50f, ObjectFieldAlignment.Left)]
		[LabelWidth(100f)]
		public Sprite palettePreview;
	}
}
