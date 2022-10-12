using System;
using DG.Tweening;
using UnityEngine;

public class IsidoraMaskLight : MonoBehaviour
{
	private void OnEnable()
	{
		base.transform.localScale = Vector3.one * this.initialRadius * 2f;
		base.transform.DOScale(this.maxRadius * 2f, this.timeToMaxRadius).SetEase(Ease.OutQuad).OnComplete(delegate
		{
			base.transform.DOScale((this.maxRadius - this.fluctuation) * 2f, this.fluctuationTime).SetDelay(0.6f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		});
	}

	private void OnDisable()
	{
		base.transform.DOKill(false);
	}

	public float initialRadius = 1f;

	public float timeToMaxRadius = 1f;

	public float maxRadius = 3f;

	public float fluctuation = 0.5f;

	public float fluctuationTime = 0.5f;
}
