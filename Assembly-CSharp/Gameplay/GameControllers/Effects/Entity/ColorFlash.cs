using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Entity
{
	public class ColorFlash : MonoBehaviour
	{
		private void Start()
		{
			try
			{
				this.flashMat = new Material(Shader.Find("Sprites/DefaultColorFlash"));
				this.flashMat.SetColor(ColorFlash.Color, this.FlashColor);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message);
			}
		}

		public void TriggerColorFlash()
		{
			if (this._waitForFlash || !this.EnabledEffect)
			{
				return;
			}
			this._waitForFlash = true;
			base.StartCoroutine(this.ColorFlashCoroutine());
		}

		private IEnumerator ColorFlashCoroutine()
		{
			this.originalMat = this.EntityRenderer.material;
			this.EntityRenderer.material = this.flashMat;
			this.EntityRenderer.material.SetFloat(ColorFlash.FlashAmount, 1f);
			yield return new WaitForSeconds(this.FlashTimeAmount);
			this.EntityRenderer.material.SetFloat(ColorFlash.FlashAmount, 0f);
			this.EntityRenderer.material = this.originalMat;
			this._waitForFlash = false;
			yield break;
		}

		public bool EnabledEffect = true;

		private bool _waitForFlash;

		[SerializeField]
		protected SpriteRenderer EntityRenderer;

		public Color FlashColor;

		private Material flashMat;

		private Material originalMat;

		public float FlashTimeAmount;

		private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

		private static readonly int Color = Shader.PropertyToID("_FlashColor");
	}
}
