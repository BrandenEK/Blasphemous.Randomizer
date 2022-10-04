using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScreenMaterialEffectsManager : MonoBehaviour
{
	private void Awake()
	{
		this.originMat = this.screenEffect.material;
	}

	private IEnumerator ScreenMaterialSwap(float duration, Material newMat)
	{
		Material oldMat = this.screenEffect.material;
		this.screenEffect.material = newMat;
		yield return new WaitForSeconds(duration);
		this.screenEffect.material = oldMat;
		yield break;
	}

	public void ScreenModeBW(float duration)
	{
		this.ResetIfNeeded();
		this.coroutine = base.StartCoroutine(this.ScreenMaterialSwap(duration, this.bwMat));
	}

	public void ScreenModeThunder(float duration)
	{
		this.ResetIfNeeded();
		this.coroutine = base.StartCoroutine(this.ScreenMaterialSwap(duration, this.thunderMat));
	}

	private void ResetIfNeeded()
	{
		if (this.coroutine != null)
		{
			base.StopCoroutine(this.coroutine);
			this.coroutine = null;
			this.screenEffect.material = this.originMat;
		}
	}

	public void SetEffect(ScreenMaterialEffectsManager.SCREEN_EFFECTS e)
	{
		if (e != ScreenMaterialEffectsManager.SCREEN_EFFECTS.NONE)
		{
			if (e == ScreenMaterialEffectsManager.SCREEN_EFFECTS.BACKLIT)
			{
				this.SetBacklit();
			}
		}
		else
		{
			this.SetOrigin();
		}
	}

	public void SetOrigin()
	{
		this.screenEffect.material = this.originMat;
	}

	public void SetBacklit()
	{
		this.screenEffect.material = this.backlitMat;
	}

	[SerializeField]
	[FoldoutGroup("References", false, 0)]
	private CustomImageEffect screenEffect;

	[SerializeField]
	[BoxGroup("Screen effects materials", true, false, 0)]
	private Material bwMat;

	[SerializeField]
	[BoxGroup("Screen effects materials", true, false, 0)]
	private Material backlitMat;

	[SerializeField]
	[BoxGroup("Screen effects materials", true, false, 0)]
	private Material thunderMat;

	private Material originMat;

	private Coroutine coroutine;

	public enum SCREEN_EFFECTS
	{
		NONE,
		BACKLIT
	}
}
