using System;
using System.Collections;
using Gameplay.UI;
using Sirenix.OdinInspector;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Entity
{
	public class MasterShaderEffects : MonoBehaviour
	{
		private void Awake()
		{
			if (this.EntityRenderer == null)
			{
				this.EntityRenderer = base.GetComponent<SpriteRenderer>();
			}
		}

		private void Start()
		{
			if (this.applyLevelEffects)
			{
				LevelInitializer levelInitializer = UnityEngine.Object.FindObjectOfType<LevelInitializer>();
				if (levelInitializer != null)
				{
					levelInitializer.ApplyLevelColorEffects(this);
				}
			}
		}

		[FoldoutGroup("Color flash", 0)]
		[Button("ColorFlash", ButtonSizes.Small)]
		public void TriggerColorFlash()
		{
			if (this._waitForFlash)
			{
				return;
			}
			this._waitForFlash = true;
			base.StartCoroutine(this.ColorFlashCoroutine());
		}

		private IEnumerator ColorFlashCoroutine()
		{
			this.EntityRenderer.material.EnableKeyword("COLOR_LERP_ON");
			this.EntityRenderer.material.SetFloat("_LerpAmount", 1f);
			yield return new WaitForSecondsRealtime(this.FlashTimeAmount);
			this.EntityRenderer.material.SetFloat("_LerpAmount", 0f);
			this.EntityRenderer.material.DisableKeyword("COLOR_LERP_ON");
			this._waitForFlash = false;
			yield break;
		}

		public void SetColorizeStrength(float v)
		{
			this.EntityRenderer.material.SetFloat("_ColorizeStrength", v);
		}

		public void SetColorTintStrength(float v)
		{
			this.EntityRenderer.material.SetFloat("_LerpAmount", v);
		}

		public void SetColorTint(Color color, float strength, bool enabled)
		{
			this.SetColorTintStrength(1f);
			this.EntityRenderer.material.SetColor("_LerpColor", color);
			if (enabled)
			{
				this.EntityRenderer.material.EnableKeyword("COLOR_LERP_ON");
			}
			else
			{
				this.EntityRenderer.material.DisableKeyword("COLOR_LERP_ON");
			}
		}

		public void TriggerColorizeLerp(float origin, float end, float seconds, Action callback = null)
		{
			base.StartCoroutine(this.ColorizeLerp(origin, end, seconds, callback));
		}

		public void TriggerColorizeLerpInOut(float secondsIn, float secondsOut)
		{
			if (this.lerpInOut != null)
			{
				base.StopCoroutine(this.lerpInOut);
			}
			this.lerpInOut = base.StartCoroutine(this.ColorizeLerpInOut(secondsIn, secondsOut));
		}

		public void TriggerColorTintLerp(float origin, float end, float seconds, Action callback = null)
		{
			base.StartCoroutine(this.ColorTintLerp(origin, end, seconds, callback));
		}

		public void StartColorizeLerp(float origin, float end, float seconds, Action callback)
		{
			base.StartCoroutine(this.ColorizeLerp(origin, end, seconds, callback));
		}

		private IEnumerator ColorizeLerp(float origin, float end, float seconds, Action callback)
		{
			this.EntityRenderer.material.EnableKeyword("COLORIZE_ON");
			float counter = 0f;
			while (counter < seconds)
			{
				float v = Mathf.Lerp(origin, end, counter / seconds);
				counter += Time.deltaTime;
				this.SetColorizeStrength(v);
				yield return null;
			}
			this.EntityRenderer.material.SetFloat("_ColorizeStrength", end);
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		private IEnumerator ColorizeLerpInOut(float secondsIn, float secondsOut)
		{
			yield return base.StartCoroutine(this.ColorizeLerp(0f, 1f, secondsIn, null));
			yield return base.StartCoroutine(this.ColorizeLerp(1f, 0f, secondsOut, null));
			yield break;
		}

		private IEnumerator ColorTintLerp(float origin, float end, float seconds, Action callback)
		{
			this.EntityRenderer.material.EnableKeyword("COLOR_LERP_ON");
			float counter = 0f;
			float v = 0f;
			while (counter < seconds)
			{
				v = Mathf.Lerp(origin, end, counter / seconds);
				counter += Time.deltaTime;
				this.SetColorTintStrength(v);
				yield return null;
			}
			this.EntityRenderer.material.SetFloat("_LerpAmount", end);
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		public void TestDamageEffect()
		{
			this.EntityRenderer.material.EnableKeyword("DAMAGE_EFFECT_ON");
			this.SetDamageEffectFromMaterial(this.damageEffectTestMaterial);
		}

		public void DamageEffectBlink(float waitSeconds, float blinkSeconds, Material effectMaterial = null)
		{
			if (effectMaterial == null)
			{
				effectMaterial = this.damageEffectTestMaterial;
			}
			this.SetDamageEffectFromMaterial(effectMaterial);
			UIController.instance.StartCoroutine(this.DamageEffectCoroutine(waitSeconds, blinkSeconds));
		}

		private IEnumerator DamageEffectCoroutine(float waitseconds, float blinkseconds)
		{
			if (this.EntityRenderer.material.name.StartsWith("ArcadeDamage"))
			{
				yield return null;
			}
			else
			{
				if (this.EntityRenderer != null)
				{
					this.EntityRenderer.material.EnableKeyword("DAMAGE_EFFECT_ON");
				}
				if (this.EntityRenderer != null)
				{
					yield return new WaitForSecondsRealtime(blinkseconds);
				}
				if (this.EntityRenderer != null)
				{
					yield return new WaitUntil(() => !this.EntityRenderer.material.name.StartsWith("ArcadeDamage"));
				}
				if (this.EntityRenderer != null)
				{
					this.EntityRenderer.material.DisableKeyword("DAMAGE_EFFECT_ON");
				}
			}
			yield break;
		}

		public void ColorizeWave(float frec)
		{
			float num = Mathf.Sin(Time.time * frec);
			num = (1f + num) / 2f;
			this.EntityRenderer.material.SetFloat("_ColorizeStrength", num);
		}

		public void SetDamageEffectFromMaterial(Material m)
		{
			this.EntityRenderer.material.SetColor("_DamageEffectColor", m.GetColor("_Color"));
			this.EntityRenderer.material.SetColor("_DamageEffectHighlight", m.GetColor("_Highlight"));
			this.EntityRenderer.material.SetFloat("_DamageEffectTimescale", m.GetFloat("_TimeScale"));
		}

		public void SetColorizeData(Color colorizeColor, Color colorizeMultColor, float colorizeAmount)
		{
			this.EntityRenderer.material.EnableKeyword("COLORIZE_ON");
			this.EntityRenderer.material.SetColor("_ColorizeColor", colorizeColor);
			this.EntityRenderer.material.SetColor("_ColorizeMultColor", colorizeMultColor);
			this.EntityRenderer.material.SetFloat("_ColorizeStrength", colorizeAmount);
		}

		public void DeactivateColorize()
		{
			this.EntityRenderer.sharedMaterial.DisableKeyword("COLORIZE_ON");
		}

		[SerializeField]
		protected SpriteRenderer EntityRenderer;

		[FoldoutGroup("Color flash", 0)]
		public Color FlashColor;

		[FoldoutGroup("Color flash", 0)]
		public float FlashTimeAmount;

		[FoldoutGroup("Damage effect", 0)]
		public Material damageEffectTestMaterial;

		public bool applyLevelEffects;

		private const string MATERIAL_TO_IGNORE = "ArcadeDamage";

		private bool _waitForFlash;

		private Coroutine lerpInOut;
	}
}
