using System;
using DG.Tweening;
using UnityEngine;

public class FloatInPlace : MonoBehaviour
{
	private void Start()
	{
		Vector2 vector = base.transform.localPosition;
		this.swayTween = base.transform.DOLocalMoveY(vector.y - this.oscillationDistance, this.loopDuration, false).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
	}

	private Tween swayTween;

	public float loopDuration;

	public float oscillationDistance;
}
