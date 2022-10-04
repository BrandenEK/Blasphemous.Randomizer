using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Recolor
{
	public class ColorPaletteSwapper : MonoBehaviour
	{
		private void Start()
		{
			this.SetMaterial();
		}

		private void SetMaterial()
		{
			Sprite currentColorPaletteSprite = Core.ColorPaletteManager.GetCurrentColorPaletteSprite();
			if (currentColorPaletteSprite != null)
			{
				SpriteRenderer component = base.GetComponent<SpriteRenderer>();
				if (component != null)
				{
					component.material.SetTexture("_PaletteTex", currentColorPaletteSprite.texture);
					if (this.extraMaterial)
					{
						this.extraMaterial.SetTexture("_PaletteTex", currentColorPaletteSprite.texture);
					}
				}
				else
				{
					Debug.LogError("There is no sprite renderer attached");
				}
			}
		}

		private const string ColorPaletteResource = "";

		public Material extraMaterial;
	}
}
