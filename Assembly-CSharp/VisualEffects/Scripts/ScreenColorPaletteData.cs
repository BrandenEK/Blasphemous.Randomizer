using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VisualEffects.Scripts
{
	[CreateAssetMenu(fileName = "Screen Color Palette", menuName = "Blasphemous/Screen Color Palette", order = 0)]
	public class ScreenColorPaletteData : ScriptableObject
	{
		[BoxGroup("Generate from Gradient", true, false, 0)]
		[ShowIf("ShouldShowGenerateButton", true)]
		[Button("Generate", ButtonSizes.Medium)]
		private void Generate()
		{
			int width = this.paletteTexture.width;
			this.paletteSize = Mathf.Min(width, this.paletteSize);
			int num = width / (this.paletteSize - 1);
			this.colors.Clear();
			for (int i = 0; i < this.paletteSize; i++)
			{
				this.colors.Add(this.paletteTexture.GetPixel(num * i + num / 2, 0));
			}
		}

		private bool ShouldShowGenerateButton()
		{
			return this.paletteTexture != null;
		}

		public void AdjustColorList()
		{
			while (this.colors.Count > this.paletteSize)
			{
				this.colors.RemoveAt(this.colors.Count - 1);
			}
		}

		public void Inject(Material mat, bool dithering)
		{
			this.colors.CopyTo(this.injectingColors);
			mat.SetColorArray("_ColorPalette", this.injectingColors);
			mat.SetInt("_PaletteSize", this.colors.Count);
			if (dithering)
			{
				mat.EnableKeyword("DITHERING");
			}
			else
			{
				mat.DisableKeyword("DITHERING");
			}
		}

		[SerializeField]
		public List<Color> colors;

		[BoxGroup("Generate from Gradient", true, false, 0)]
		public Texture2D paletteTexture;

		[BoxGroup("Generate from Gradient", true, false, 0)]
		[Range(4f, 32f)]
		public int paletteSize = 8;

		private const int MAX_PALETTE_SIZE = 32;

		private readonly Color[] injectingColors = new Color[32];
	}
}
