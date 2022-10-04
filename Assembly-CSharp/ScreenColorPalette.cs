using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VisualEffects.Scripts;

[ExecuteInEditMode]
public class ScreenColorPalette : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.material == null)
		{
			this.material = Resources.Load<Material>("Materials/Effects_Nostalgia");
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (this.material == null || this.currentPalette == null)
		{
			Graphics.Blit(src, dest);
		}
		else
		{
			if (this.lastInjectedPalette == null || this.lastInjectedPalette != this.currentPalette)
			{
				this.currentPalette.Inject(this.material, this.dithering);
				this.lastInjectedPalette = this.currentPalette;
			}
			Graphics.Blit(src, dest, this.material);
		}
	}

	[BoxGroup("Effects", true, false, 0)]
	public bool dithering;

	[BoxGroup("Palettes", true, false, 0)]
	public List<ScreenColorPaletteData> availablePalettes;

	[BoxGroup("Palettes", true, false, 0)]
	[ValueDropdown("availablePalettes")]
	[ShowInInspector]
	private ScreenColorPaletteData currentPalette;

	private ScreenColorPaletteData lastInjectedPalette;

	private Material material;
}
