using System;
using DG.Tweening;
using UnityEngine;

public class IsidoraMaskLight : MonoBehaviour
{
	private void OnEnable()
	{
		base.transform.localScale = Vector3.one * this.initialRadius * 2f;
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(base.transform, this.maxRadius * 2f, this.timeToMaxRadius), 6), delegate()
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(ShortcutExtensions.DOScale(base.transform, (this.maxRadius - this.fluctuation) * 2f, this.fluctuationTime), 0.6f), 7), -1, 1);
		});
	}

	private void OnDisable()
	{
		ShortcutExtensions.DOKill(base.transform, false);
	}

	public float initialRadius = 1f;

	public float timeToMaxRadius = 1f;

	public float maxRadius = 3f;

	public float fluctuation = 0.5f;

	public float fluctuationTime = 0.5f;
}
